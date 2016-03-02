// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2015, SIL International. All Rights Reserved.
// <copyright from='2011' to='2015' company='SIL International'>
//		Copyright (c) 2015, SIL International. All Rights Reserved.
//
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using HearThis.Communication;
using HearThis.Properties;
using HearThis.Publishing;
using HearThis.Script;
using L10NSharp;
using NetSparkle;
using SIL.IO;
using SIL.Windows.Forms.Miscellaneous;
using SIL.Windows.Forms.ReleaseNotes;
using Paratext;
using Utilities;
using SIL.DblBundle.Text;
using SIL.Reporting;

namespace HearThis.UI
{
	public partial class Shell : Form
	{
		public static Sparkle UpdateChecker;
		public event EventHandler OnProjectChanged;
		private string _projectNameToShow = string.Empty;

#if MULTIPLEMODES
		private List<string> allowableModes;

		private const string kAdministrative = "Administrator";
#endif
		private const string kNormalRecording = "NormalRecording";

		public Shell()
		{
			InitializeComponent();
			Text = Program.kProduct;

			_settingsProtectionHelper.ManageComponent(toolStripButtonSettings);
			_settingsProtectionHelper.ManageComponent(toolStripButtonChooseProject);
			SetupUILanguageMenu();

			_toolStrip.Renderer = new RecordingToolControl.NoBorderToolStripRenderer();
			toolStripButtonAbout.ForeColor = AppPallette.NavigationTextColor;

			InitializeModesCombo();

			UpdateChecker = new Sparkle(
				@"http://build.palaso.org/guestAuth/repository/download/bt90/.lastSuccessful/appcast.xml",
				(System.Drawing.Icon) (new ComponentResourceManager(this.GetType()).GetObject("$this.Icon")));
			UpdateChecker.CheckOnFirstApplicationIdle();
			// Todo: possibly make this conditional on an a device being connected.
			// If possible notice and show it when a device is later connected.
			// Or: possibly if no device is active it displays instructions.
			toolStripButtonSyncAndroid.Visible = true;
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
					// ENHANCE: Someday it might be nice to save/restore these in a project file so they could be remembered on
					// a per-project basis, but the VAST majority of our users are going to be working on a single project, so
					// this might be good enough.
					Settings.Default.Book = -1;
					Settings.Default.Chapter = -1;
					Settings.Default.Block = -1;
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
				Program.LocalizationManager.ShowLocalizationDialogBox(false);
				SetupUILanguageMenu();
			});
		}

		private void InitializeModesCombo()
		{
			_btnMode.DropDownItems.Clear();
#if MULTIPLEMODES
			allowableModes = new List<string>();
			if (Settings.Default.AllowAdministrativeMode)
			{
				ToolStripItem item = _btnMode.DropDownItems.Add(LocalizationManager.GetString("MainWindow.Modes.Administrator",
					"Administrator"));
				item.Tag = kAdministrative;
				if (Settings.Default.ActiveMode == kAdministrative)
					SetMode(item);
			}
			if (Settings.Default.AllowNormalRecordingMode)
			{
				ToolStripItem item = _btnMode.DropDownItems.Add(LocalizationManager.GetString("MainWindow.Modes.NormalRecording",
					"Normal Recording"));
				item.Tag = kNormalRecording;
				if (Settings.Default.ActiveMode == kNormalRecording)
					SetMode(item);
			}
#endif
			_btnMode.Visible = (_btnMode.DropDownItems.Count > 1);
			_recordingToolControl1.HidingSkippedBlocks = Settings.Default.ActiveMode == kNormalRecording;
		}

#if MULTIPLEMODES
		private void SetMode(ToolStripItem selectedMode)
		{
			_btnMode.Text = selectedMode.Text;

			switch ((string)selectedMode.Tag)
			{
				case kAdministrative:
					_recordingToolControl1.HidingSkippedBlocks = false;
					break;
				case kNormalRecording:
					_recordingToolControl1.HidingSkippedBlocks = true;
					break;
			}
			SetWindowText();
		}
#endif

		private void OnSaveClick(object sender, EventArgs e)
		{
			MessageBox.Show(
				LocalizationManager.GetString("MainWindow.SaveAutomatically",
					"HearThis automatically saves your work, while you use it. This button is just here to tell you that :-)  To create sound files for playing your recordings, click the Publish button."),
				LocalizationManager.GetString("Common.Save", "Save"));
		}

		private void OnPublishClick(object sender, EventArgs e)
		{
			using (var dlg = new PublishDialog(Project))
			{
				dlg.ShowDialog();
			}
		}

		private void OnSettingsButtonClicked(object sender, EventArgs e)
		{
			var origBreakQuotesIntoBlocksValue = Settings.Default.BreakQuotesIntoBlocks;
			var origAdditionalBlockBreakChars = Settings.Default.AdditionalBlockBreakCharacters;
			DialogResult result = _settingsProtectionHelper.LaunchSettingsIfAppropriate(() =>
			{
				using (var dlg = new AdministrativeSettings(Project))
				{
					return dlg.ShowDialog(FindForm());
				}
			});
			if (result == DialogResult.OK)
			{
				if (origBreakQuotesIntoBlocksValue != Settings.Default.BreakQuotesIntoBlocks ||
					origAdditionalBlockBreakChars != Settings.Default.AdditionalBlockBreakCharacters)
				{
					LoadProject(Settings.Default.Project);
				}
				else
				{
					ScriptControl.ScriptBlockPainter.SetClauseSeparators();
#if MULTIPLEMODES
					Invoke(new Action(InitializeModesCombo));
#else
					Invoke(new Action(() =>
					{
						_recordingToolControl1.HidingSkippedBlocks = Settings.Default.ActiveMode == kNormalRecording;
					}));
#endif
				}
			}
		}

		private void OnAboutClick(object sender, EventArgs e)
		{
			using (var dlg = new SILAboutBox(FileLocator.GetFileDistributedWithApplication("aboutbox.htm")))
			{
				dlg.CheckForUpdatesClicked += HandleAboutDialogCheckForUpdatesClick;
				dlg.ReleaseNotesClicked += HandleAboutDialogReleaseNotesClicked;
				dlg.ShowDialog();
			}
		}

		private void HandleAboutDialogReleaseNotesClicked(object sender, EventArgs e)
		{
			var path = FileLocator.GetFileDistributedWithApplication("ReleaseNotes.md");
			using (var dlg = new ShowReleaseNotesDialog(((Form)sender).Icon, path))
				dlg.ShowDialog();
		}

		private void HandleAboutDialogCheckForUpdatesClick(object sender, EventArgs e)
		{
			var updateStatus = UpdateChecker.CheckForUpdatesAtUserRequest();
			if (updateStatus == Sparkle.UpdateStatus.UpdateNotAvailable)
				((SILAboutBox)sender).NotifyNoUpdatesAvailable();
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
				_projectNameToShow = name;
				ScriptProviderBase scriptProvider;
				if (name == SampleScriptProvider.kProjectUiName)
					scriptProvider = new SampleScriptProvider();
				else if (Path.GetExtension(name) == ExistingProjectsList.kProjectFileExtension ||
					Path.GetExtension(name) == ".zip")
				{
					TextBundle<DblTextMetadata<DblMetadataLanguage>, DblMetadataLanguage> bundle;
					try
					{
						bundle = new TextBundle<DblTextMetadata<DblMetadataLanguage>, DblMetadataLanguage>(name);
					}
					catch (Exception e)
					{
						ErrorReport.ReportNonFatalExceptionWithMessage(e,
							LocalizationManager.GetString("MainWindow.ProjectMetadataInvalid", "Project could not be loaded: {0}"), name);
						return false;
					}
					var metadata = bundle.Metadata;

					var hearThisProjectFolder = Path.Combine(Program.ApplicationDataBaseFolder, metadata.Language.Iso + "_" + metadata.Name);

					if (Path.GetExtension(name) == ".zip" || Path.GetDirectoryName(name) != hearThisProjectFolder)
					{
						var projectFile = Path.Combine(hearThisProjectFolder, Path.ChangeExtension(Path.GetFileName(name), ExistingProjectsList.kProjectFileExtension));
						if (Directory.Exists(hearThisProjectFolder))
						{
							if (File.Exists(projectFile))
							{
								//TODO: Deal with collision. Offer to open existing project. Overwrite using this bundle?
								return false;
							}
						}
						else
							Directory.CreateDirectory(hearThisProjectFolder);
						File.Copy(name, projectFile);
						name = projectFile;
						bundle = new TextBundle<DblTextMetadata<DblMetadataLanguage>, DblMetadataLanguage>(name);
					}
					scriptProvider = new ParatextScriptProvider(new TextBundleScripture(bundle));
					_projectNameToShow = metadata.Name;
				}
				else
				{
					ScrText paratextProject = ScrTextCollection.Get(name);
					if (paratextProject == null)
						return false;
					_projectNameToShow = paratextProject.JoinedNameAndFullName;
					scriptProvider = new ParatextScriptProvider(new ParatextScripture(paratextProject));
				}

				Project = new Project(scriptProvider);
				if (OnProjectChanged != null)
					OnProjectChanged(this, new EventArgs());
				SetWindowText();

				Settings.Default.Project = name;
				Settings.Default.Save();
				return true;
			}
			catch (Exception e)
			{
				ErrorReport.NotifyUserOfProblem(e, "Could not open " + name);
			}
			return false; //didn't load it
		}

		private void SetWindowText()
		{
			var ver = Assembly.GetExecutingAssembly().GetName().Version;
#if MULTIPLEMODES
			Text =
				string.Format(
					LocalizationManager.GetString("MainWindow.WindowTitle", "{3} -- HearThis {0}.{1}.{2} ({4})",
						"{3} is project name, {0}.{1}.{2} are parts of version number. {4} is the active mode (i.e., view)"),
						ver.Major, ver.Minor, ver.Build, _projectNameToShow, _btnMode.Text);
#else
			Text =
				string.Format(
					LocalizationManager.GetString("MainWindow.WindowTitle", "{3} -- {4} {0}.{1}.{2}",
						"{4} is product name: HearThis; {3} is project name, {0}.{1}.{2} are parts of version number."),
						ver.Major, ver.Minor, ver.Build, _projectNameToShow, Program.kProduct);
#endif
		}

		private void ModeDropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
#if MULTIPLEMODES
			if (Settings.Default.ActiveMode != (string) e.ClickedItem.Tag)
			{
				Settings.Default.ActiveMode = (string) e.ClickedItem.Tag;
				SetMode(e.ClickedItem);
			}
#endif
		}

		private void toolStripButtonSyncAndroid_Click(object sender, EventArgs e)
		{
			AndroidSynchronization.DoAndroidSync(Project);
		}
	}
}