using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HearThis.Properties;
using Microsoft.Win32;
using Palaso.Reporting;
using Paratext;

namespace HearThis.UI
{
	public partial class ChooseProject : Form
	{
		public ChooseProject()
		{
			InitializeComponent();
		}

		private void GetProjectChoices()
		{
			var key = @"HKEY_LOCAL_MACHINE\SOFTWARE\ScrChecks\1.0\Settings_Directory";
			var path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\ScrChecks\1.0\Settings_Directory", "", null);
			if (path == null || !Directory.Exists(path.ToString()))
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
					if (!text.IsResourceText || text.Name == "GNTUK")
					{
						_projectsList.Items.Add(text);

//                        MenuItem x = new MenuItem(text.Name);
//                        x.Tag = text;
//                        x.Click += new EventHandler(OnSelectProjectClick);
//                        _recordingToolControl1.ContextMenu.MenuItems.Add(x);
					}
				}

			}
			catch (Exception err)
			{
				var result = ErrorReport.NotifyUserOfProblem(new ShowAlwaysPolicy(), "Quit", DialogResult.Abort,
															 "There was a problem starting up access to Paratext Files. If you are just checking out HearThis and don't have Paratext installed.  Click OK, and we'll set you up with a pretend text.\r\nThe error was: {0}",
															 err.Message);

				if (result == DialogResult.Abort)
					Application.Exit();

				//TODO: set up with pretend project
			}
		}

		private void ChooseProject_Load(object sender, EventArgs e)
		{
			GetProjectChoices();
		}

		private void _projectsList_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			_okButton.Enabled = _projectsList.SelectedIndex > -1;
		}
	}

}
