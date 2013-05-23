using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using HearThis.Properties;
using HearThis.Script;
using L10NSharp;
using NetSparkle;
using Palaso.Progress;
using Paratext;

namespace HearThis.UI
{
	public partial class Shell : Form
	{
		public static Sparkle UpdateChecker;

		public Shell()
		{
			InitializeComponent();
			_recordingToolControl1.ChooseProject += new EventHandler(OnChooseProject);
			SetWindowText("");

			UpdateChecker = new Sparkle(@"http://build.palaso.org/guestAuth/repository/download/bt90/.lastSuccessful/appcast.xml", (System.Drawing.Icon)(new ComponentResourceManager(this.GetType()).GetObject("$this.Icon")));
			UpdateChecker.CheckOnFirstApplicationIdle();
		}

		void OnChooseProject(object sender, EventArgs e)
		{
			ChooseProject();
		}

		private bool ChooseProject()
		{
			using (var dlg = new ChooseProject())
			{
				if (DialogResult.OK == dlg.ShowDialog())
				{
					LoadProject(dlg.SelectedProject.Name);
					return true;
				}
				return false;
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			bool loaded = false;
			if (!string.IsNullOrEmpty(Settings.Default.Project) )
			{
				loaded = LoadProject(Settings.Default.Project);
			}

			if(!loaded) //if never did have a project, or that project couldn't be loaded
			{
				if(!ChooseProject())
					Close();
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
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
				Project project;
				var nameToShow = name;
				if (name == "Sample")
				{
					project = new Project("sample", new SampleScriptProvider());
				}
				else
				{
					ScrText paratextProject = Paratext.ScrTextCollection.Get(name);
					if (paratextProject == null)
						return false;
					nameToShow = paratextProject.JoinedNameAndFullName;
					var paratextScriptProvider = new ParatextScriptProvider(new Scripture(paratextProject));
					var progressState = new ProgressState();
					progressState.NumberOfStepsCompletedChanged += new EventHandler(progressState_NumberOfStepsCompletedChanged);
					//paratextScriptProvider.LoadBible(progressState);
					project = new Project(name, paratextScriptProvider);
				}
					_recordingToolControl1.SetProject(project);
					SetWindowText(nameToShow);

					Settings.Default.Project = name;
					Settings.Default.Save();
					return true;
			}
			catch (Exception e)
			{
				Palaso.Reporting.ErrorReport.NotifyUserOfProblem(e, "Could not open " + Settings.Default.Project);
			}
			return false; //didn't load it
		}

		void progressState_NumberOfStepsCompletedChanged(object sender, EventArgs e)
		{
			Debug.WriteLine(((ProgressState)sender).NumberOfStepsCompleted);
		}

		private void SetWindowText(string projectName)
		{
			var ver = Assembly.GetExecutingAssembly().GetName().Version;
			Text = string.Format(LocalizationManager.GetString("MainWindow.WindowTitle", "{3} -- HearThis {0}.{1}.{2}", "{3} is project name, {0}.{1}.{2} are parts of version number"), ver.Major, ver.Minor, ver.Build, projectName);
		}
	}
}
