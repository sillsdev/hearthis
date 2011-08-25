using System;
using System.Reflection;
using System.Windows.Forms;
using HearThis.Properties;
using HearThis.Script;
using Paratext;

namespace HearThis.UI
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			_recordingToolControl1.ChooseProject += new EventHandler(OnChooseProject);
			SetWindowText("");
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

		private bool LoadProject(string name)
		{
			try
			{
				ScrText paratextProject = Paratext.ScrTextCollection.Get(name);
				if (paratextProject == null)
					return false;
				var project = new Project(name, new ParatextScriptProvider(paratextProject));

				_recordingToolControl1.SetProject(project);
				SetWindowText(name);
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

		private void SetWindowText(string projectName)
		{
			var ver = Assembly.GetExecutingAssembly().GetName().Version;
			Text = string.Format("{3} -- HearThis {0}.{1}.{2}", ver.Major, ver.Minor, ver.Build, projectName);
		}
	}
}
