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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DesktopAnalytics;
using HearThis.Properties;
using HearThis.Script;
using L10NSharp;
using Microsoft.Win32;
using SIL.Reporting;
using Paratext;
using SIL.Windows.Forms.PortableSettingsProvider;
using Utilities;
using Platform = SIL.PlatformUtilities.Platform;

namespace HearThis.UI
{
	public partial class ChooseProject : Form
	{
		private readonly SampleScriptProvider _sampleScriptProvider = new SampleScriptProvider();
		public ChooseProject()
		{
			InitializeComponent();

			if (Settings.Default.ChooseProjectFormSettings == null)
				Settings.Default.ChooseProjectFormSettings = FormSettings.Create(this);

			_projectsList.SelectedProject = Settings.Default.Project;
			_projectsList.GetParatextProjects = GetParatextProjects;
			_projectsList.SampleProjectInfo = _sampleScriptProvider;
		}

		private IEnumerable<ScrText> GetParatextProjects()
		{
			ScrText[] paratextProjects = null;
			try
			{
				paratextProjects = ScrTextCollection.ScrTexts(false, false).ToArray();
			}
			catch (Exception err)
			{
				NotifyUserOfParatextProblem(LocalizationManager.GetString("ChooseProject.CantAccessParatext",
					"There was a problem accessing Paratext data files."),
					string.Format(LocalizationManager.GetString("ChooseProject.ParatextError", "The error was: {0}"), err.Message));
				paratextProjects = new ScrText[0];
			}
			if (paratextProjects.Any())
			{
				_lblNoParatextProjectsInFolder.Visible = false;
				_lblNoParatextProjects.Visible = false;
			}
			else
			{
				if (Program.ParatextIsInstalled)
					_lblNoParatextProjects.Visible = true;
				else
				{
					_lblNoParatextProjectsInFolder.Visible = _tableLayoutPanelParatextProjectsFolder.Visible;
				}
			}
			return paratextProjects;
		}

		protected override void OnLoad(EventArgs e)
		{
			Settings.Default.ChooseProjectFormSettings.InitializeForm(this);
			_projectsList.GridSettings = Settings.Default.ChooseProjectGridSettings;

			base.OnLoad(e);

			if (!Program.ParatextIsInstalled)
			{
				if (String.IsNullOrWhiteSpace(Settings.Default.UserSpecifiedParatextProjectsDir))
				{
					_lblParatextNotInstalled.Visible = true;
					_linkFindParatextProjectsFolder.Visible = true;
				}
				else
				{
					_tableLayoutPanelParatextProjectsFolder.Visible = true;
					_lblParatextProjectsFolder.Text = ScrTextCollection.SettingsDirectory;
				}
			}

			UpdateDisplay();
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			if (IsParatext8Installed)
			{
				const string downloadUrl = "http://software.sil.org/hearthis/download/";
				var msgFmt = LocalizationManager.GetString("ChooseProject.Paratext8RequiresHT15",
					"It looks like {0} is installed on this computer. To access {0} projects, you will need to install {1} or " +
					"later from\n{2}\nThis is not an automatic upgrade.",
					"Param 0: \"Paratext 8\"; Param 1: \"HearThis 1.5\"; Param 2: \"http://software.sil.org/hearthis/download/\"");
				MessageBox.Show(this, String.Format(msgFmt, "Paratext 8", ProductName + " 1.5", downloadUrl), ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			Settings.Default.ChooseProjectGridSettings = _projectsList.GridSettings;
			base.OnClosing(e);
		}

		private void NotifyUserOfParatextProblem(string message, params string[] additionalInfo)
		{
			additionalInfo.Aggregate(message, (current, s) => current + ("\r\n" + s));

			var result = ErrorReport.NotifyUserOfProblem(new ShowAlwaysPolicy(),
				LocalizationManager.GetString("Common.Quit", "Quit"), ErrorResult.Abort, message);

			if (result == ErrorResult.Abort)
				Application.Exit();
		}

		private void _projectsList_SelectedProjectChanged(object sender, EventArgs e)
		{
			UpdateDisplay();
		}

		private static bool IsParatext8Installed
		{
			get
			{
				const string settingsKey32 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Paratext\8";
				const string settingsKey64 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Paratext\8";
				var p8RegistryKey = Environment.Is64BitProcess && Platform.IsWindows ? settingsKey64 : settingsKey32;
				var path = Registry.GetValue(p8RegistryKey, "Settings_Directory", null);
				return path != null && Directory.Exists(path.ToString());
			}
		}

		public string SelectedProject { get; private set; }

		private void UpdateDisplay()
		{
			_okButton.Enabled = !string.IsNullOrEmpty(_projectsList.SelectedProject);
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
			DialogResult = DialogResult.OK;
			Analytics.Track("SetProject");
			Close();
		}

		private void _projectsList_DoubleClick(object sender, EventArgs e)
		{
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
				var defaultFolder = Settings.Default.UserSpecifiedParatextProjectsDir;
#if !__MonoCS__
				if (String.IsNullOrWhiteSpace(defaultFolder))
					defaultFolder = @"c:\My Paratext Projects";
#endif
				if (!String.IsNullOrWhiteSpace(defaultFolder) && Directory.Exists(defaultFolder))
					dlg.SelectedPath = defaultFolder;

				dlg.Description = LocalizationManager.GetString("ChooseProject.FindParatextProjectsFolder",
					"Find Paratext projects folder", "Displayed in folder browser dialog (only accessible if Paratext is not installed).");
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					try
					{
						ScrTextCollection.Initialize(dlg.SelectedPath);
					}
					catch (Exception ex)
					{
						var msg = String.Format(LocalizationManager.GetString("ChooseProject.ErrorSettingParatextProjectsFolder",
							"An error occurred trying to set Paratext projects location to:\r\n{0}Error message:\r\n{0}"),
							dlg.SelectedPath, ex.Message);
						Analytics.Track("ErrorSettingParatextProjectsFolder",
							new Dictionary<string, string> { {"Error", ex.ToString()} });
						MessageBox.Show(msg, ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
						return;
					}
					Settings.Default.UserSpecifiedParatextProjectsDir = ScrTextCollection.SettingsDirectory;
					_lblParatextNotInstalled.Visible = false;
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
					SelectedProject = dlg.FileName;
					DialogResult = DialogResult.OK;
					Close();
				}
			}
		}
	}
}
