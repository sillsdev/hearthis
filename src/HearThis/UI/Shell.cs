using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using HearThis.Properties;
using HearThis.Publishing;
using HearThis.Script;
using L10NSharp;
using NetSparkle;
using Palaso.Progress;
using Palaso.UI.WindowsForms.SettingProtection;
using Paratext;

namespace HearThis.UI
{
	public partial class Shell : Form
	{
		public static Sparkle UpdateChecker;
		public event EventHandler OnProjectChanged;
		private List<string> allowableModes;

		private const string kAdministrative = "Administrative";
		private const string kNormalRecording = "NormalRecording";

		public SettingsProtectionHelper SettingsProtectionHelper
		{
			get { return _settingsProtectionHelper; }
		}

		public Shell()
		{
			InitializeComponent();
			_settingsProtectionHelper.ManageComponent(toolStripButtonSettings);
			SetupUILanguageMenu();

			SetWindowText("");

			_toolStrip.Renderer = new RecordingToolControl.NoBorderToolStripRenderer();
			toolStripButton4.ForeColor = AppPallette.NavigationTextColor;

			InitializeModesCombo();

			UpdateChecker = new Sparkle(
				@"http://build.palaso.org/guestAuth/repository/download/bt90/.lastSuccessful/appcast.xml",
				(System.Drawing.Icon) (new ComponentResourceManager(this.GetType()).GetObject("$this.Icon")));
			UpdateChecker.CheckOnFirstApplicationIdle();
		}

		public Project Project { get; private set; }

		private void OnChooseProject(object sender, EventArgs e)
		{
			ChooseProject();
		}

		private bool ChooseProject()
		{
			using (var dlg = new ChooseProject())
			{
				if (DialogResult.OK == dlg.ShowDialog())
				{
					LoadProject(dlg.SelectedProject);
					return true;
				}
				return false;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			bool loaded = false;
			if (!string.IsNullOrEmpty(Settings.Default.Project))
			{
				loaded = LoadProject(Settings.Default.Project);
			}

			if (!loaded) //if never did have a project, or that project couldn't be loaded
			{
				if (!ChooseProject())
					Close();
			}
		}

		private void SetupUILanguageMenu()
		{
			_uiLanguageMenu.DropDownItems.Clear();
			foreach (var lang in LocalizationManager.GetUILanguages(true))
			{
				var item = _uiLanguageMenu.DropDownItems.Add(lang.NativeName);
				item.Tag = lang;
				item.Click += ((a, b) =>
				{
					LocalizationManager.SetUILanguage(((CultureInfo) item.Tag).IetfLanguageTag, true);
					Settings.Default.UserInterfaceLanguage = ((CultureInfo) item.Tag).IetfLanguageTag;
					item.Select();
					_uiLanguageMenu.Text = ((CultureInfo) item.Tag).NativeName;
				});
				if (((CultureInfo) item.Tag).IetfLanguageTag == Settings.Default.UserInterfaceLanguage)
				{
					_uiLanguageMenu.Text = ((CultureInfo) item.Tag).NativeName;
				}
			}

			_uiLanguageMenu.DropDownItems.Add(new ToolStripSeparator());
			var menu = _uiLanguageMenu.DropDownItems.Add(LocalizationManager.GetString("MainWindow.MoreMenuItem",
				"More...", "Last item in menu of UI languages"));
			menu.Click += ((a, b) =>
			{
				LocalizationManager.ShowLocalizationDialogBox(this);
				SetupUILanguageMenu();
			});
		}

		private void InitializeModesCombo()
		{
			_cboMode.Items.Clear();
			int index;
			allowableModes = new List<string>();
			if (Settings.Default.AllowAdministrativeMode)
			{
				index = _cboMode.Items.Add(LocalizationManager.GetString("MainWindow.Modes.Administrative",
					"Administrative Mode"));
				allowableModes.Add(kAdministrative);
				if (Settings.Default.ActiveMode == kAdministrative)
				{
					_cboMode.SelectedIndex = index;
					_recordingToolControl1.HidingSkippedBlocks = false;
				}
			}
			if (Settings.Default.AllowNormalRecordingMode)
			{
				index = _cboMode.Items.Add(LocalizationManager.GetString("MainWindow.Modes.ANormalRecordingdministrative",
						"Normal Recording Mode"));
				allowableModes.Add(kNormalRecording);
				if (Settings.Default.ActiveMode == kNormalRecording)
				{
					_cboMode.SelectedIndex = index;
					_recordingToolControl1.HidingSkippedBlocks = true;
				}
			}

			_settingsProtectionHelper.SetSettingsProtection(_cboMode.Control, _cboMode.Items.Count == 1);
		}

		private void OnSaveClick(object sender, EventArgs e)
		{
			MessageBox.Show(
				LocalizationManager.GetString("MainWindow.SaveAutomatically",
					"HearThis automatically saves your work, while you use it. This button is just here to tell you that :-)  To create sound files for playing your recordings, click the Publish button."),
				LocalizationManager.GetString("Common.Save", "Save"));
		}

		private void OnPublishClick(object sender, EventArgs e)
		{
			using (var dlg = new PublishDialog(new PublishingModel(Project.Name, Project.EthnologueCode)))
			{
				dlg.ShowDialog();
			}
		}

		private void OnSettingsButtonClicked(object sender, EventArgs e)
		{
			DialogResult result = _settingsProtectionHelper.LaunchSettingsIfAppropriate(() =>
			{
				using (var dlg = new AdministrativeSettings())
				{
					return dlg.ShowDialog(FindForm());
				}
			});
			if (result == DialogResult.OK)
			{
				Invoke(new Action(InitializeModesCombo));
			}
		}

		private void OnAboutClick(object sender, EventArgs e)
		{
			using (var dlg = new AboutDialog())
			{
				dlg.ShowDialog();
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			Settings.Default.Save();
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			_recordingToolControl1.StartFilteringMessages();
		}

		protected override void OnDeactivate(EventArgs e)
		{
			base.OnDeactivate(e);
			_recordingToolControl1.StopFilteringMessages();
		}

		private bool LoadProject(string name)
		{
			try
			{
				var nameToShow = name;
				IScriptProvider scriptProvider;
				if (name == SampleScriptProvider.kProjectUiName)
					scriptProvider = new SampleScriptProvider();
				else
				{
					ScrText paratextProject = ScrTextCollection.Get(name);
					if (paratextProject == null)
						return false;
					nameToShow = paratextProject.JoinedNameAndFullName;
					scriptProvider = new ParatextScriptProvider(new Scripture(paratextProject));
					var progressState = new ProgressState();
					progressState.NumberOfStepsCompletedChanged += progressState_NumberOfStepsCompletedChanged;
				}

				Project = new Project(scriptProvider);
				if (OnProjectChanged != null)
					OnProjectChanged(this, new EventArgs());
				SetWindowText(nameToShow);

				Settings.Default.Project = name;
				Settings.Default.Save();
				return true;
			}
			catch (Exception e)
			{
				Palaso.Reporting.ErrorReport.NotifyUserOfProblem(e, "Could not open " + name);
			}
			return false; //didn't load it
		}

		private void progressState_NumberOfStepsCompletedChanged(object sender, EventArgs e)
		{
			Debug.WriteLine(((ProgressState) sender).NumberOfStepsCompleted);
		}

		private void SetWindowText(string projectName)
		{
			var ver = Assembly.GetExecutingAssembly().GetName().Version;
			Text =
				string.Format(
					LocalizationManager.GetString("MainWindow.WindowTitle", "{3} -- HearThis {0}.{1}.{2}",
						"{3} is project name, {0}.{1}.{2} are parts of version number"), ver.Major, ver.Minor, ver.Build, projectName);
		}

		private void SelectedModeChanged(object sender, EventArgs e)
		{
			if (_cboMode.SelectedIndex < 0)
				return;

			Settings.Default.ActiveMode = allowableModes[_cboMode.SelectedIndex];
			switch (Settings.Default.ActiveMode)
			{
				case kAdministrative:
					_recordingToolControl1.HidingSkippedBlocks = false;
					break;
				case kNormalRecording:
					_recordingToolControl1.HidingSkippedBlocks = true;
					break;
			}
		}
	}
}