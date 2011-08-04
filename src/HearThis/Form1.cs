using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using HearThis.Properties;
using Microsoft.Win32;
using Palaso.Reporting;
using Paratext;

namespace HearThis
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			_recordingToolControl1.ContextMenu = new ContextMenu();


			var key = @"HKEY_LOCAL_MACHINE\SOFTWARE\ScrChecks\1.0\Settings_Directory";
			var path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\ScrChecks\1.0\Settings_Directory", "", null);
			if(path==null || !Directory.Exists(path.ToString()))
			{
			   var result = ErrorReport.NotifyUserOfProblem(new ShowAlwaysPolicy(), "Quit", DialogResult.Abort,
												"It looks like this computer doesn't have Paratext installed. If you are just checking out HearThis, then click OK, and we'll set you up with some pretend text.");

				if (result == DialogResult.Abort)
					Application.Exit();

				Settings.Default.Project = "Sample";
			}

			try
			{
			   ScrTextCollection.Initialize();

				foreach (var text in Paratext.ScrTextCollection.ScrTexts)
				{
					if (!text.IsResourceText)
					{
						MenuItem x = new MenuItem(text.Name);
						x.Tag = text;
						x.Click += new EventHandler(OnSelectProjectClick);
						_recordingToolControl1.ContextMenu.MenuItems.Add(x);
					}
				}

			}
			catch(Exception err)
			{
				var result = ErrorReport.NotifyUserOfProblem(new ShowAlwaysPolicy(), "Quit", DialogResult.Abort,
												"There was a problem starting up access to Paratext Files. If you are just checking out HearThis and don't have Paratext installed.  Click OK, and we'll set you up with a pretend text.\r\nThe error was: {0}", err.Message);

				if (result == DialogResult.Abort)
					Application.Exit();

				//TODO: set up with pretend project
			}


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
					project = new Project(name, new SampleTextProvider());
				}
				else
				{
					project = new Project(name, new ParatextTextProvider(Paratext.ScrTextCollection.Get(name)));
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
