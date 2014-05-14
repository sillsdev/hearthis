using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using L10NSharp;
using Microsoft.Win32;
using Palaso.Reporting;
using Paratext;

namespace HearThis.UI
{
	public partial class ChooseProject : Form
	{
		const string ParaTExtRegistryKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\ScrChecks\1.0\Settings_Directory";

		public ChooseProject()
		{
			InitializeComponent();
		}

		protected override void  OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			var path = Registry.GetValue(ParaTExtRegistryKey, "", null);
			if (path == null || !Directory.Exists(path.ToString()))
			{
				var result = ErrorReport.NotifyUserOfProblem(new ShowAlwaysPolicy(), LocalizationManager.GetString("Common.Quit", "Quit"), ErrorResult.Abort,
					LocalizationManager.GetString("ChooseProject.NoParatext",
					"It looks like this computer doesn't have Paratext installed. If you are just checking out HearThis, click OK, and we'll set you up with some pretend text."));
				if (result == ErrorResult.Abort)
					Application.Exit();

				UseSampleProject();
			}

			try
			{
				_projectsList.Items.AddRange(ScrTextCollection.ScrTexts(false, false).ToArray<object>());
			}
			catch (Exception err)
			{
				var result = ErrorReport.NotifyUserOfProblem(new ShowAlwaysPolicy(), LocalizationManager.GetString("Common.Quit", "Quit"), ErrorResult.Abort,
															  LocalizationManager.GetString("ChooseProject.CantAccessParatext",
															  "There was a problem accessing Paratext data files. If you are just checking out HearThis and don't have Paratext installed, click OK, and we'll set you up with a pretend text.\r\nThe error was: {0}"),
															 err.Message);

				if (result == ErrorResult.Abort)
					Application.Exit();

				UseSampleProject();
			}

			UpdateDisplay();
		}

		private void UseSampleProject()
		{
			SelectedProject = "Sample";
			DialogResult = DialogResult.OK;
			Close();
		}

		private void _projectsList_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateDisplay();
			SelectedProject = ((ScrText)_projectsList.SelectedItem).Name;
		}

		public string SelectedProject { get; set; }

		private void UpdateDisplay()
		{
			_okButton.Enabled = _projectsList.SelectedIndex > -1;
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;

			SelectedProject = null;
			Close();
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			UsageReporter.SendNavigationNotice("SetProject");
			Close();
		}

		private void _projectsList_DoubleClick(object sender, EventArgs e)
		{
			_okButton_Click(this,null);
		}
	}

}
