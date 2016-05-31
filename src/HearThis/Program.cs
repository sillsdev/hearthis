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
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using DesktopAnalytics;
using HearThis.Properties;
using HearThis.Script;
using HearThis.UI;
using L10NSharp;
using Microsoft.Win32;
using SIL.IO;
using SIL.Reporting;
using Paratext;
using SIL.WritingSystems;

namespace HearThis
{
	internal static class Program
	{
		private static string _sHearThisFolder;

		private const string ParaTExtRegistryKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\ScrChecks\1.0\Settings_Directory";
		public const string kCompany = "SIL";
		public const string kProduct = "HearThis";
		private static List<Exception> _pendingExceptionsToReportToAnalytics = new List<Exception>();

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// The following not only gets the location of the settings file; it also
			// detects corruption and deletes it if needed so HearThis doesn't crash.
			var userConfigSettingsPath = GetUserConfigFilePath();

			if ((Control.ModifierKeys & Keys.Shift) > 0 && !string.IsNullOrEmpty(userConfigSettingsPath))
			{
				var confirmationString = LocalizationManager.GetString("Program.ConfirmDeleteUserSettingsFile",
					"Do you want to delete your user settings? (This will clear your most-recently-used project, publishing settings, UI language settings, etc. It will not affect your HearThis project data.)");

				if (DialogResult.Yes ==
					MessageBox.Show(confirmationString, kProduct, MessageBoxButtons.YesNo, MessageBoxIcon.Warning))
				{
					File.Delete(userConfigSettingsPath);
				}
			}

			//bring in settings from any previous version
			if (Settings.Default.NeedUpgrade)
			{
				//see http://stackoverflow.com/questions/3498561/net-applicationsettingsbase-should-i-call-upgrade-every-time-i-load
				Settings.Default.Upgrade();
				Settings.Default.NeedUpgrade = false;
				Settings.Default.Save();
			}

			SetUpErrorHandling();
			SetupLocalization();

			if (args.Length == 1 && args[0].Trim() == "-afterInstall")
			{
				using (var dlg = new SIL.Windows.Forms.ReleaseNotes.ShowReleaseNotesDialog(Resources.HearThis,  FileLocator.GetFileDistributedWithApplication( "releaseNotes.md")))
				{
					dlg.ShowDialog();
				}
			}

			string lastName = null;
			string emailAddress = null;

			if (Control.ModifierKeys == Keys.Control)
			{
				Settings.Default.Project = SampleScriptProvider.kProjectUiName;
			}
			else if (ParatextIsInstalled)
			{

				try
				{
					ScrTextCollection.Initialize();
					var regData = RegistrationInfo.RegistrationData;
					lastName = regData.Name;
					emailAddress = regData.Email;
				}
				catch (Exception ex)
				{
					_pendingExceptionsToReportToAnalytics.Add(ex);
					// Later we'll notify the user that Paratext is not properly installed, and they'll have a chance to
					// try to initialize using an alternate location. Rather than defaulting to Sample, that will just be one
					// of the choices (possibly the only choice) in the list.
					//ErrorReport.NotifyUserOfProblem(
					//	LocalizationManager.GetString("Program.ParatextNotInstalled",
					//		"It looks like perhaps Paratext is not installed on this computer, or there is some other problem connecting to it. We'll set you up with a sample so you can play with HearThis, but you'll have to install Paratext to get any real work done here.",
					//		""));

					//Settings.Default.Project = SampleScriptProvider.kProjectUiName;
				}
			}
			else if (!String.IsNullOrWhiteSpace(Settings.Default.UserSpecifiedParatextProjectsDir) && Directory.Exists(Settings.Default.UserSpecifiedParatextProjectsDir))
			{
				try
				{
					ScrTextCollection.Initialize(Settings.Default.UserSpecifiedParatextProjectsDir);
				}
				catch (Exception ex)
				{
					_pendingExceptionsToReportToAnalytics.Add(ex);
					Settings.Default.UserSpecifiedParatextProjectsDir = null;
				}
			}

			string firstName = null;
			if (lastName != null)
			{
				var split = lastName.LastIndexOf(" ", StringComparison.Ordinal);
				if (split > 0)
				{
					firstName = lastName.Substring(0, split);
					lastName = lastName.Substring(split + 1);
				}
			}
			var userInfo = new UserInfo { FirstName = firstName, LastName = lastName, UILanguageCode = LocalizationManager.UILanguageId, Email = emailAddress};

#if DEBUG
			// Always track if this is a debug build, but track to a different segment.io project
			const bool allowTracking = true;
			const string key = "pldi6z3n3vfz23czhano";
#else
			// If this is a release build, then allow an environment variable to be set to false
			// so that testers aren't generating false analytics
			string feedbackSetting = System.Environment.GetEnvironmentVariable("FEEDBACK");

			var allowTracking = string.IsNullOrEmpty(feedbackSetting) || feedbackSetting.ToLower() == "yes" || feedbackSetting.ToLower() == "true";

			const string key = "bh7aaqmlmd0bhd48g3ye";
#endif
			using (new Analytics(key, userInfo, allowTracking))
			{
				foreach (var exception in _pendingExceptionsToReportToAnalytics)
					Analytics.ReportException(exception);
				_pendingExceptionsToReportToAnalytics.Clear();

				Sldr.Initialize();

				try
				{
					Application.Run(new Shell());
				}
				finally
				{
					Sldr.Cleanup();
				}
			}
		}

		public static bool ParatextIsInstalled
		{
			get
			{
				var path = Registry.GetValue(ParaTExtRegistryKey, "", null);
				return path != null && Directory.Exists(path.ToString());
			}
		}

		public static string GetUserConfigFilePath()
		{
			try
			{
				return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
			}
			catch (System.Configuration.ConfigurationErrorsException e)
			{
				_pendingExceptionsToReportToAnalytics.Add(e);
				File.Delete(e.Filename);
				return e.Filename;
			}
		}

		public static LocalizationManager LocalizationManager { get; private set; }

		private static void SetupLocalization()
		{
			var installedStringFileFolder = FileLocator.GetDirectoryDistributedWithApplication("localization");
			var targetTmxFilePath = Path.Combine(kCompany, kProduct);
			string desiredUiLangId = Settings.Default.UserInterfaceLanguage;
			LocalizationManager = LocalizationManager.Create(desiredUiLangId, "HearThis", Application.ProductName, Application.ProductVersion,
				installedStringFileFolder, targetTmxFilePath, Resources.HearThis, IssuesEmailAddress, "HearThis");
			LocalizationManager.Create(LocalizationManager.UILanguageId, "Palaso", "Palaso", Application.ProductVersion, installedStringFileFolder,
									   targetTmxFilePath, Resources.HearThis, IssuesEmailAddress, "SIL.Windows.Forms.DblBundle");
		}

		/// <summary>
		/// The email address people should write to with problems (or new localizations?) for HearThis.
		/// </summary>
		public static string IssuesEmailAddress
		{
			get { return "issues@hearthis.palaso.org"; }
		}

		/// ------------------------------------------------------------------------------------
		private static void SetUpErrorHandling()
		{
			ErrorReport.EmailAddress = "issues@hearthis.palaso.org";
			ErrorReport.AddStandardProperties();
			ExceptionHandler.Init();
			ExceptionHandler.AddDelegate(ReportError);
		}

		private static void ReportError(object sender, CancelExceptionHandlingEventArgs e)
		{
			Analytics.ReportException(e.Exception);
		}

		#region AppData folder structure
		/// <summary>
		/// Get the folder %AppData%/SIL/HearThis where we store recordings and localization stuff.
		/// </summary>
		public static string ApplicationDataBaseFolder
		{
			get
			{
				if (_sHearThisFolder == null)
				{
					_sHearThisFolder = Utils.CreateDirectory(
						Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
						Program.kCompany, Program.kProduct);
				}
				return _sHearThisFolder;
			}
		}

		/// <summary>
		/// Create (if necessary) and return the requested subfolder of the HearThis base AppData folder.
		/// </summary>
		/// <param name="projectName"></param>
		public static string GetApplicationDataFolder(string projectName)
		{
			return Utils.CreateDirectory(ApplicationDataBaseFolder, projectName);
		}
		#endregion
	}
}
