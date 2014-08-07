using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using HearThis.Script;
using L10NSharp;
using Microsoft.Win32;
using Palaso.Reporting;
using Paratext;

namespace HearThis.UI
{
	public partial class ChooseProject : Form
	{
		private const string ParaTExtRegistryKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\ScrChecks\1.0\Settings_Directory";

		public ChooseProject()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			var path = Registry.GetValue(ParaTExtRegistryKey, "", null);
			if (path == null || !Directory.Exists(path.ToString()))
			{
				NotifyUserOfParatextProblemAndOfferSampleProject(LocalizationManager.GetString("ChooseProject.NoParatext",
					"It looks like this computer doesn't have Paratext installed."));
			}

			try
			{
				_projectsList.Items.AddRange(ScrTextCollection.ScrTexts(false, false).ToArray<object>());
				if (_projectsList.Items.Count == 0)
				{
					NotifyUserOfParatextProblemAndOfferSampleProject(LocalizationManager.GetString("ChooseProject.NoParatextProjects",
						"No Paratext user projects were found."));
				}
			}
			catch (Exception err)
			{
				NotifyUserOfParatextProblemAndOfferSampleProject(LocalizationManager.GetString("ChooseProject.CantAccessParatext",
					"There was a problem accessing Paratext data files."),
					string.Format(LocalizationManager.GetString("ChooseProject.ParatextError", "The error was: {0}"), err.Message));
			}

			UpdateDisplay();
		}

		private void NotifyUserOfParatextProblemAndOfferSampleProject(string message, params string[] additionalInfo)
		{
			message += "\r\n" + LocalizationManager.GetString("ChooseProject.ClickOkForSampleText",
				"If you are just checking out HearThis, click OK, and we'll set you up with some pretend text.");
			additionalInfo.Aggregate(message, (current, s) => current + ("\r\n" + s));

			var result = ErrorReport.NotifyUserOfProblem(new ShowAlwaysPolicy(),
				LocalizationManager.GetString("Common.Quit", "Quit"), ErrorResult.Abort, message);

			if (result == ErrorResult.Abort)
				Application.Exit();

			UseSampleProject();
		}

		private void UseSampleProject()
		{
			SelectedProject = SampleScriptProvider.kProjectUiName;
			DialogResult = DialogResult.OK;
			Close();
		}

		private void _projectsList_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateDisplay();
			ScrText selectedText = (ScrText) _projectsList.SelectedItem;
			SelectedProject = selectedText != null ? selectedText.Name : null;
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
			_okButton_Click(this, null);
		}
	}
}
