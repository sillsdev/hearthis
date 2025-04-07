// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2011-2025, SIL Global.
// <copyright from='2011' to='2025' company='SIL Global'>
//		Copyright (c) 2011-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DesktopAnalytics;
using HearThis.Properties;
using HearThis.Script;
using L10NSharp;
using SIL.Reporting;
using Paratext.Data;
using SIL.Windows.Forms.PortableSettingsProvider;
using static System.String;
using static HearThis.UI.ChooseProject.ParatextLoadErrorStrings;

namespace HearThis.UI
{
	public partial class ChooseProject : Form, ILocalizable
	{
		private readonly SampleScriptProvider _sampleScriptProvider = new SampleScriptProvider(true);

		public ChooseProject()
		{
			InitializeComponent();

			if (Settings.Default.ChooseProjectFormSettings == null)
				Settings.Default.ChooseProjectFormSettings = FormSettings.Create(this);

			_projectsList.SelectedProject = Settings.Default.Project;
			_projectsList.GetParatextProjects = GetParatextProjects;
			_projectsList.SampleProjectInfo = _sampleScriptProvider;

			Program.RegisterLocalizable(this);
			HandleStringsLocalized();
		}

		public void HandleStringsLocalized()
		{
			_lblParatextNotInstalled.Text = Format(_lblParatextNotInstalled.Text,
				"Paratext", "8", "9");
		}

		// Note: This method is very similar to the method by the same name in the OpenProjectDlg
		// class in Glyssen. If improvements are made here, they should also be made there if
		// applicable.
		private IEnumerable<ScrText> GetParatextProjects()
		{
			ScrText[] paratextProjects = null;
			try
			{
				paratextProjects = ScrTextCollection.ScrTexts(IncludeProjects.AccessibleScripture).ToArray();
				var loadErrors = Program.CompatibleParatextProjectLoadErrors.ToList();
				if (loadErrors.Any())
				{
					var sb = new StringBuilder(ParatextProjectLoadErrors);
					foreach (var errMsgInfo in loadErrors)
					{
						sb.Append("\n\n");
						try
						{
							switch (errMsgInfo.Reason)
							{
								case UnsupportedReason.UnknownType:
									AppendVersionIncompatibilityMessage(sb, errMsgInfo);
									sb.AppendFormat(UnknownProjectType,
										errMsgInfo.ProjecType);
									break;

								case UnsupportedReason.CannotUpgrade:
									// HearThis is newer than project version
									AppendVersionIncompatibilityMessage(sb, errMsgInfo);
									sb.AppendFormat(ProjectOutdated,
										ParatextInfo.MinSupportedParatextDataVersion, Program.kProduct);
									break;

								case UnsupportedReason.FutureVersion:
									// Project version is newer than HearThis
									AppendVersionIncompatibilityMessage(sb, errMsgInfo);
									sb.AppendFormat(HearThisVersionOutdated,
										Program.kProduct,
										ScrTextCollection.ScrTexts(IncludeProjects.Everything).First(
											p => p.Name == errMsgInfo.ProjectName).Settings.MinParatextDataVersion);
									break;

								case UnsupportedReason.Corrupted:
								case UnsupportedReason.Unspecified:
									sb.AppendFormat(Generic,
										errMsgInfo.ProjectName, errMsgInfo.Exception.Message);
									break;

								default:
									throw errMsgInfo.Exception;
							}
						}
						catch (Exception e)
						{
							ErrorReport.ReportNonFatalException(e);
						}
					}
					MessageBox.Show(sb.ToString(), Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
			catch (Exception err)
			{
				NotifyUserOfParatextProblem(CantAccessParatext,
					Format(ParatextError, err.Message));
				paratextProjects = Array.Empty<ScrText>();
			}
			if (paratextProjects.Any())
			{
				_lblNoParatextProjectsInFolder.Visible = false;
				_lblNoParatextProjects.Visible = false;
			}
			else
			{
				if (ParatextInfo.IsParatextInstalled)
					_lblNoParatextProjects.Visible = true;
				else
				{
					_lblNoParatextProjectsInFolder.Visible = _tableLayoutPanelParatextProjectsFolder.Visible;
				}
			}
			return paratextProjects;
		}

		private static void AppendVersionIncompatibilityMessage(StringBuilder sb, ErrorMessageInfo errMsgInfo)
		{
			sb.AppendFormat(LocalizationManager.GetString("ChooseProject.ParatextProjectLoadError.VersionIncompatibility",
					"Project {0} is not compatible with this version of {1}.",
					"Param 0: Paratext project name; Param 1: \"HearThis\""),
				errMsgInfo.ProjectName, Program.kProduct).Append(' ');
		}

		protected override void OnLoad(EventArgs e)
		{
			Settings.Default.ChooseProjectFormSettings.InitializeForm(this);
			_projectsList.GridSettings = Settings.Default.ChooseProjectGridSettings;

			base.OnLoad(e);

			if (!ParatextInfo.IsParatextInstalled)
			{
				if (IsNullOrWhiteSpace(Settings.Default.UserSpecifiedParatext8ProjectsDir))
				{
					if (ParatextInfo.IsParatext7Installed)
						_lblParatext7Installed.Visible = true;
					else
						_lblParatextNotInstalled.Visible = true;
					_linkFindParatextProjectsFolder.Visible = true;
				}
				else
				{
					_tableLayoutPanelParatextProjectsFolder.Visible = true;
					// In this case, during program startup, the ScrTextCollection will have already been
					// initialized to the user-specified folder.
					_lblParatextProjectsFolder.Text = ScrTextCollection.SettingsDirectory;
				}
			}

			UpdateDisplay();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (_lblNoParatextProjectsInFolder.Visible)
			{
				// Probably no point saving this, if they chose a folder where there were no projects.
				Settings.Default.UserSpecifiedParatext8ProjectsDir = null;
			}
			Settings.Default.ChooseProjectGridSettings = _projectsList.GridSettings;
			base.OnClosing(e);
		}

		private void NotifyUserOfParatextProblem(string message, params string[] additionalInfo)
		{
			additionalInfo.Aggregate(message, (current, s) => current + Environment.NewLine + s);

			var result = ErrorReport.NotifyUserOfProblem(new ShowAlwaysPolicy(),
				LocalizationManager.GetString("ChooseProject.Quit", "Quit"), ErrorResult.Abort, message);

			if (result == ErrorResult.Abort)
				Application.Exit();
		}

		private void _projectsList_SelectedProjectChanged(object sender, EventArgs e)
		{
			UpdateDisplay();
		}

		public string SelectedProject { get; private set; }

		private void UpdateDisplay()
		{
			_okButton.Enabled = !IsNullOrEmpty(_projectsList.SelectedProject);
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;

			SelectedProject = null;
			Close();
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			SelectedProject = _projectsList.SelectedProject;
			var paratextProjectId = _projectsList.IdentifierForParatextProject;
				if (!IsNullOrEmpty(paratextProjectId))
					SelectedProject += "." + paratextProjectId;
				DialogResult = DialogResult.OK;
				Analytics.Track("SetProject");
				Close();
			}

		private void _projectsList_DoubleClick(object sender, EventArgs e)
		{
			if (_okButton.Enabled)
				_okButton_Click(this, null);
		}

		private void _linkFindParatextProjectsFolder_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			FindParatextProjectsFolder();
		}

		private void HandleFindParatextProjectsFolderButtonClicked(object sender, EventArgs e)
		{
			FindParatextProjectsFolder();
		}

		private void FindParatextProjectsFolder()
		{
			using (var dlg = new FolderBrowserDialog())
			{
				dlg.ShowNewFolderButton = false;
				var defaultFolder = Settings.Default.UserSpecifiedParatext8ProjectsDir;

				if (IsNullOrWhiteSpace(defaultFolder) || !Directory.Exists(defaultFolder))
					defaultFolder = Empty;
#if !__MonoCS__
				if (defaultFolder == Empty)
				{
					defaultFolder = new[]
					{
						@"c:\My Paratext 8 Projects",
						@"c:\My Paratext 9 Projects",
						@"c:\My Paratext Projects"
					}.FirstOrDefault(Directory.Exists) ?? Empty;
				}
#endif
				dlg.SelectedPath = defaultFolder;

				dlg.Description = LocalizationManager.GetString("ChooseProject.FindParatextProjectsFolder",
					"Find Paratext projects folder", "Displayed in folder browser dialog (only accessible if Paratext is not installed).");
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					try
					{
						ParatextData.Initialize(dlg.SelectedPath);
					}
					catch (Exception ex)
					{
						var msg = Format(LocalizationManager.GetString("ChooseProject.ErrorSettingParatextProjectsFolder",
							"An error occurred trying to set Paratext projects location to:\n{0}"),
							dlg.SelectedPath);
						Analytics.Track("ErrorSettingParatextProjectsFolder",
							new Dictionary<string, string> { {"Error", ex.ToString()} });
						// For any problem inside ParatextData (other than ApplicationException), we want a report with
						// the call stack so we can follow up with the Paratext team if needed.
						if (ex is ApplicationException)
						{
							msg += Environment.NewLine +
								LocalizationManager.GetString("ChooseProject.ErrorSettingPTProjFolderExceptionDetailsLabel", "Error message:") +
								Environment.NewLine + ex.Message;
							MessageBox.Show(msg, ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						}
						else
							ErrorReport.ReportNonFatalExceptionWithMessage(ex, msg);
						return;
					}
					Settings.Default.UserSpecifiedParatext8ProjectsDir = ScrTextCollection.SettingsDirectory;
					_lblParatextNotInstalled.Visible = false;
					_lblParatext7Installed.Visible = false;
					_tableLayoutPanelParatextProjectsFolder.Visible = true;
					_linkFindParatextProjectsFolder.Visible = false;
					_lblParatextProjectsFolder.Text = ScrTextCollection.SettingsDirectory;
					_projectsList.ReloadExistingProjects();
					UpdateDisplay();
				}
			}
		}

		private void _linkCreateFromBundle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			using (var dlg = new SelectBundleDlg())
			{
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					Analytics.Track("ProjectCreatedFromBundle");
					SelectedProject = dlg.FileName;
					DialogResult = DialogResult.OK;
					Close();
				}
			}
		}

		private void _linkCreateFromGlyssenScript_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			using (var dlg = new OpenFileDialog())
			{
				dlg.Filter = @"GlyssenScript files (*" + MultiVoiceScriptProvider.kMultiVoiceFileExtension + @")|*" +
				             MultiVoiceScriptProvider.kMultiVoiceFileExtension;
				dlg.RestoreDirectory = true;
				dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					Analytics.Track("ProjectCreatedFromGlyssenScript");
					SelectedProject = dlg.FileName;
					DialogResult = DialogResult.OK;
					Close();
				}
			}
		}

		/// <summary>
		/// This class exists merely as a convenience to avoid having the localized strings in
		/// methods that cannot be processed by the extraction code. See note in l10n.proj.
		/// </summary>
		public static class ParatextLoadErrorStrings
		{
			public static string CantAccessParatext =>
				LocalizationManager.GetString("ChooseProject.CantAccessParatext",
					"There was a problem accessing Paratext data files.");

			public static string Generic =>
				LocalizationManager.GetString("ChooseProject.ParatextProjectLoadError.Generic",
					"Project: {0}\nError message: {1}", "Param 0: Paratext project name; Param 1: error details");

			public static string HearThisVersionOutdated =>
				LocalizationManager.GetString("ChooseProject.ParatextProjectLoadError.HearThisVersionOutdated",
					"To read this project, a version of {0} compatible with Paratext {1} is required.",
					"Param 0: \"HearThis\"; Param 1: Paratext version number");

			public static string ParatextError =>
				LocalizationManager.GetString("ChooseProject.ParatextError", "The error was: {0}");

			public static string ProjectOutdated =>
				LocalizationManager.GetString("ChooseProject.ParatextProjectLoadError.ProjectOutdated",
					"The project administrator needs to update it by opening it with Paratext {0} or later. " +
					"Alternatively, you might be able to revert to an older version of {1}.",
					"Param 0: Paratext version number; Param 1: \"HearThis\"");

			public static string ParatextProjectLoadErrors =>
				LocalizationManager.GetString("ChooseProject.ParatextProjectLoadErrors",
					"The following Paratext project load errors occurred:");

			public static string UnknownProjectType =>
				LocalizationManager.GetString("ChooseProject.ParatextProjectLoadError.UnknownProjectType",
					"This project has a project type ({0}) that is not supported.", "Param 0: Paratext project type");
		}
	}
}
