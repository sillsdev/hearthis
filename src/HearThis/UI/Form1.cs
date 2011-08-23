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

			_recordingToolControl1.ContextMenu = new ContextMenu();

			SetWindowText("");
		}

		void OnSelectProjectClick(object sender, EventArgs e)
		{
			var paratextProject = ((ScrText)((MenuItem)sender).Tag);
			LoadProject(paratextProject.Name);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(Settings.Default.Project))
			{
				LoadProject(Settings.Default.Project);
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			Settings.Default.Save();
		}

		private void LoadProject(string name)
		{
			try
			{
				Project project;
				if(string.IsNullOrEmpty(name) || name=="Sample")
				{
					project = new Project(name, new SampleScriptProvider());
				}
				else
				{
					project = new Project(name, new ParatextScriptProvider(Paratext.ScrTextCollection.Get(name)));
				}
				_recordingToolControl1.SetProject(project);
				SetWindowText(name);
				Settings.Default.Project = name;
				Settings.Default.Save();
			}
			catch (Exception e)
			{
				Palaso.Reporting.ErrorReport.NotifyUserOfProblem(e, "Could not open " + Settings.Default.Project);
			}
		}

		private void SetWindowText(string projectName)
		{
			var ver = Assembly.GetExecutingAssembly().GetName().Version;
			Text = string.Format("{3} -- HearThis {0}.{1}.{2}", ver.Major, ver.Minor, ver.Build, projectName);
		}
	}
}
