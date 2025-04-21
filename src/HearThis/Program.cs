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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DesktopAnalytics;
using HearThis.Properties;
using HearThis.Script;
using HearThis.UI;
using L10NSharp;
using L10NSharp.XLiffUtils;
using L10NSharp.UI;
using SIL.IO;
using SIL.Reporting;
using Paratext.Data;
using Paratext.Data.Users;
using PtxUtils;
using SIL.Windows.Forms.LocalizationIncompleteDlg;
using SIL.Windows.Forms.Reporting;
using SIL.Windows.Forms.SettingProtection;
using SIL.WritingSystems;
using static System.IO.Path;
using static HearThis.FileContentionHelper;
using System.Threading;
using static System.String;
using static System.Windows.Forms.MessageBoxButtons;
using static HearThis.Script.MultiVoiceScriptProvider;

namespace HearThis
{
	internal static class Program
	{
		private static string _sHearThisFolder;
		private static UserInfo s_userInfo;

		private const string kCompany = "SIL";
		public const string kProduct = "HearThis";
		public const string kSupportUrlSansHttps = "community.scripture.software.sil.org/c/hearthis";
		internal const string kLocalizationFolder = "localization";
		private static readonly List<Exception> _pendingExceptionsToReportToAnalytics = new List<Exception>();
		private static readonly HashSet<ILocalizable> s_registeredLocalizableObjects = new HashSet<ILocalizable>();

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			var launchedFromInstaller = args.Any(a => a.Trim() == "-afterInstall");
			var showReleaseNotes = false;

			using (var upgradeMutex = new Mutex(false, UpgradeMutexName, out bool createdNew))
			{
				if (!createdNew)
				{
					bool gotMutex = false;
					// If another instance is upgrading, try to wait a little bit for it to finish.
					try
					{
						upgradeMutex.WaitOne(TimeSpan.FromSeconds(5));
					}
					catch (AbandonedMutexException)
					{
						// Mutex was abandoned by another process. We can consider this as owning it.
						gotMutex = true;
					}

					if (!gotMutex)
					{
						var upgradeMsg = Format(LocalizationManager.GetString(
							"Program.WaitForUpgrade",
							"{0} is upgrading. Please wait for the first instance of {0} " +
							"to finish starting up before trying to run another instance.",
							"Param is \"HearThis\" (product name)"), kProduct);
						MessageBox.Show(upgradeMsg, kProduct, OK, MessageBoxIcon.Warning);
						return;
					}
				}

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);

				// The following not only gets the location of the settings file; it also
				// detects corruption and deletes it if needed so HearThis doesn't crash.
				var userConfigSettingsPath = GetUserConfigFilePath();

				if ((Control.ModifierKeys & Keys.Shift) > 0 && !IsNullOrEmpty(userConfigSettingsPath))
				{
					var confirmationString = Format(LocalizationManager.GetString(
						"Program.ConfirmDeleteUserSettingsFile",
						"Do you want to delete your user settings? (This will clear your most " +
						"recently used project, publishing settings, UI language settings, etc. " +
						"It will not affect your {0} project data.)",
						"Param is \"HearThis\" (product name)"), kProduct);

					if (DialogResult.Yes ==
						MessageBox.Show(confirmationString, kProduct, YesNo, MessageBoxIcon.Warning))
					{
						RobustFile.Delete(userConfigSettingsPath);
					}
				}

				// Bring in settings from any previous version
				if (Settings.Default.NeedUpgrade)
				{
					//see https://stackoverflow.com/questions/3498561/net-applicationsettingsbase-should-i-call-upgrade-every-time-i-load
					Settings.Default.Upgrade();
					Settings.Default.NeedUpgrade = false;
					// If this is the first run of this version of HearThis and it was launched from
					// the installer, we want to show the release notes.
					// ENHANCE: Ideally, we probably only want to do this for major/minor version
					// changes (or only if there's a new entry in Release notes since the last version).
					// REVIEW: Do we want to display the release notes on first launch even if not
					// from the installer?
					showReleaseNotes = launchedFromInstaller;
					Settings.Default.Save();
				}

				if (Settings.Default.RestartingToChangeColorScheme)
				{
					RestartedToChangeColorScheme = true;
					Settings.Default.RestartingToChangeColorScheme = false;
				}
				else
				{
					// There are a couple settings that we always revert to default on restart
					// unless restarting due to a color scheme change.

					// This is an advanced admin setting - we revert it as a safety measure.
					Settings.Default.AllowDisplayOfShiftClipsMenu = false;

					// In case a (possibly previous?) user had the project open in another
					// mode, we reset to the default mode to avoid confusion.
					Settings.Default.CurrentMode = Mode.ReadAndRecord;
				}

				Settings.Default.Save();
			}
			
			SetUpErrorHandling();
			Logger.Init();
			SettingsProtectionSingleton.ProductSupportUrl = kSupportUrlSansHttps;
			SetupLocalization();

			string userName = null;
			string emailAddress = null;

			if (Control.ModifierKeys == Keys.Control)
				Settings.Default.Project = SampleScriptProvider.kProjectUiName;
			else if (args.Length == 1 && GetExtension(args[0]).ToLowerInvariant() == kMultiVoiceFileExtension)
				Settings.Default.Project = args[0];

			Alert.Implementation = new AlertImpl(); // Do this before calling Initialize, just in case Initialize tries to display an alert.
			if (ParatextInfo.IsParatextInstalled)
			{
				try
				{
					ParatextData.Initialize();
					userName = RegistrationInfo.DefaultUser.Name;
					emailAddress = RegistrationInfo.DefaultUser.EmailAddress;
					foreach (var errMsgInfo in CompatibleParatextProjectLoadErrors.Where(e => e.Reason == UnsupportedReason.Unspecified))
					{
						_pendingExceptionsToReportToAnalytics.Add(errMsgInfo.Exception);
					}
				}
				catch (Exception fatalEx) when (fatalEx is FileLoadException || fatalEx is TypeInitializationException)
				{
					ErrorReport.ReportFatalException(fatalEx);
				}
				catch (Exception ex)
				{
					_pendingExceptionsToReportToAnalytics.Add(ex);
				}
			}
			else
			{
				RegistrationInfo.Implementation = new HearThisAnonymousRegistrationInfo();

				if (!IsNullOrWhiteSpace(Settings.Default.UserSpecifiedParatext8ProjectsDir) &&
					Directory.Exists(Settings.Default.UserSpecifiedParatext8ProjectsDir))
				{
					try
					{
						ParatextData.Initialize(Settings.Default.UserSpecifiedParatext8ProjectsDir);
					}
					catch (Exception ex)
					{
						_pendingExceptionsToReportToAnalytics.Add(ex);
						Settings.Default.UserSpecifiedParatext8ProjectsDir = null;
					}
				}
			}

			string firstName = null, lastName = null;
			if (userName != null)
			{
				var split = userName.LastIndexOf(" ", StringComparison.Ordinal);
				if (split > 0)
				{
					firstName = userName.Substring(0, split);
					lastName = userName.Substring(split + 1);
				}
				else
				{
					lastName = userName;
				}
			}
			s_userInfo = new UserInfo { FirstName = firstName, LastName = lastName, UILanguageCode = LocalizationManager.UILanguageId, Email = emailAddress};

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
			using (new Analytics(key, s_userInfo, allowTracking))
			{
				foreach (var exception in _pendingExceptionsToReportToAnalytics)
					Analytics.ReportException(exception);
				_pendingExceptionsToReportToAnalytics.Clear();

				if (!Sldr.IsInitialized)
					Sldr.Initialize();
				Icu.Wrapper.ConfineIcuVersions(70);
				try
				{
					var mainWindow = new Shell(launchedFromInstaller, showReleaseNotes);
					mainWindow.ProjectLoadInitializationSequenceCompleted +=
						delegate { RestartedToChangeColorScheme = false; };
					Application.Run(mainWindow);
				}
				finally
				{
					Sldr.Cleanup();
				}
			}
		}

		public static bool RestartedToChangeColorScheme { get; private set; }

		public static IEnumerable<ErrorMessageInfo> CompatibleParatextProjectLoadErrors => ScrTextCollection.ErrorMessages.Where(e => e.ProjecType != ProjectType.Resource && !e.ProjecType.IsNoteType());

		private static string GetUserConfigFilePath()
		{
			try
			{
				return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
			}
			catch (ConfigurationErrorsException e)
			{
				_pendingExceptionsToReportToAnalytics.Add(e);
				RobustFile.Delete(e.Filename);
				return e.Filename;
			}
		}

		internal static LocalizationIncompleteViewModel LocIncompleteViewModel { get; private set; }

		public static ILocalizationManager PrimaryLocalizationManager => LocIncompleteViewModel.PrimaryLocalizationManager;

		public static void RegisterLocalizable(ILocalizable uiElement)
		{
			lock (s_registeredLocalizableObjects)
			{
				if (!s_registeredLocalizableObjects.Any())
					LocalizeItemDlg<XLiffDocument>.StringsLocalized += HandleStringsLocalized;
				s_registeredLocalizableObjects.Add(uiElement);
				if (uiElement is Control ctrl) // Currently this is always true
					ctrl.Disposed += DisposingLocalizableUiElement;
			}
		}

		private static void DisposingLocalizableUiElement(object sender, EventArgs e)
		{
			// Technically, this is probably pointless
			((Control)sender).Disposed -= DisposingLocalizableUiElement;
			UnregisterLocalizable((ILocalizable)sender);
		}

		// This could be made public and be called directly if ever we had an object that
		// implemented ILocalizable but was not a Control.
		private static void UnregisterLocalizable(ILocalizable uiElement)
		{
			lock (s_registeredLocalizableObjects)
			{
				s_registeredLocalizableObjects.Remove(uiElement);
				if (!s_registeredLocalizableObjects.Any())
					LocalizeItemDlg<XLiffDocument>.StringsLocalized -= HandleStringsLocalized;
			}
		}

		private static void HandleStringsLocalized(ILocalizationManager lm)
		{
			if (lm == PrimaryLocalizationManager)
			{
				lock (s_registeredLocalizableObjects)
				{
					foreach (var handler in s_registeredLocalizableObjects)
						handler.HandleStringsLocalized();
				}
			}
		}

		private static void SetupLocalization()
		{
			var installedStringFileFolder = FileLocationUtilities.GetDirectoryDistributedWithApplication(kLocalizationFolder);
			var relativeSettingPathForLocalizationFolder = Combine(kCompany, kProduct);
			string desiredUiLangId = Settings.Default.UserInterfaceLanguage;
			Logger.WriteEvent("Initial desired UI language: " + desiredUiLangId);
			// ENHANCE (L10nSharp): Not sure what the best way is to deal with this: the desired UI
			// language might be available in the XLIFF files for one of the localization managers
			// but not the other. Normally, part of the creation process for a LM is to check to
			// see whether the requested language is available. But if the first LM we create does
			// not have the requested language, the user sees a dialog box alerting them to that
			// and requiring them to choose a different language. For now, in HearThis, we can work
			// around that by creating the Palaso LM first, since its set of available languages is
			// a superset of the languages available for HearThis. But it feels weird not to create
			// the primary LM first, and the day could come where neither set of languages is a
			// superset, and then this strategy wouldn't work.
			LocalizationManager.Create(desiredUiLangId, "Palaso", "Palaso", Application.ProductVersion, installedStringFileFolder,
				relativeSettingPathForLocalizationFolder, Resources.HearThis, IssuesEmailAddress, new [] {"SIL.Windows.Forms", "SIL.DblBundle"},
				typeof(SIL.Localizer)
					.GetMethods(BindingFlags.Static | BindingFlags.Public)
					.Where(m => m.Name == "GetString"));
			if (desiredUiLangId != LocalizationManager.UILanguageId)
			{
				Logger.WriteEvent($"Palaso did not have {desiredUiLangId}. Fallback UI language: {LocalizationManager.UILanguageId}");
				Settings.Default.UserInterfaceLanguage = LocalizationManager.UILanguageId;
			}

			var primaryMgr = LocalizationManager.Create(desiredUiLangId, "HearThis.exe", Application.ProductName, Application.ProductVersion,
				installedStringFileFolder, relativeSettingPathForLocalizationFolder, Resources.HearThis, IssuesEmailAddress, new [] {"HearThis"});
			LocIncompleteViewModel = new LocalizationIncompleteViewModel(primaryMgr, "hearthis", IssueRequestForLocalization);
			Logger.WriteEvent("Initial UI language: " + LocalizationManager.UILanguageId);
		}

		private static void IssueRequestForLocalization()
		{
			Analytics.Track("UI language request", LocIncompleteViewModel.StandardAnalyticsInfo);
		}

		/// <summary>
		/// The email address people should write to with problems (or new localizations?) for HearThis.
		/// </summary>
		private static string IssuesEmailAddress => "hearthis_issues@sil.org";

		/// ------------------------------------------------------------------------------------
		private static void SetUpErrorHandling()
		{
			ErrorReport.EmailAddress = IssuesEmailAddress;
			ErrorReport.AddStandardProperties();
			ExceptionHandler.Init(new WinFormsExceptionHandler());
			ExceptionHandler.AddDelegate(ReportError);
			ExceptionHandler.AddDelegate(ConditionallyIgnoreConfigurationErrorsException);
		}

		private static void ReportError(object sender, CancelExceptionHandlingEventArgs e)
		{
			Analytics.ReportException(e.Exception);
		}

		#region AppData folder structure
		/// <summary>
		/// Get the folder %AppData%/SIL/HearThis where we store clips and localization stuff.
		/// </summary>
		public static string ApplicationDataBaseFolder
		{
			get
			{
				if (_sHearThisFolder == null)
				{
					_sHearThisFolder = Utils.CreateDirectory(
						Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
						kCompany, kProduct);
				}
				return _sHearThisFolder;
			}
		}

		/// <summary>
		/// Create (if necessary) and return the requested subfolder of the HearThis base AppData folder.
		/// </summary>
		/// <param name="projectName">The project name. This will be used as part of a file path;
		/// caller is responsible for ensuring that it does not contain characters that would make
		/// it an invalid directory name. It should be considered as case-sensitive (although on
		/// Windows, directory names are not case-sensitive).
		/// </param>
		public static string GetApplicationDataFolder(string projectName)
		{
			return Utils.CreateDirectory(GetPossibleApplicationDataFolder(projectName));
		}

		public static string GetPossibleApplicationDataFolder(string projectName)
		{
			return Combine(ApplicationDataBaseFolder, projectName);
		}
		#endregion

		#region HearThisAnonymousRegistrationInfo class
		/// <summary>
		/// Implementation of <see cref="RegistrationInfo"/> used to allow access to local Paratext projects when Paratext is not installed
		/// </summary>
		private sealed class HearThisAnonymousRegistrationInfo : RegistrationInfo
		{
			protected override bool AcceptLicense(UserLicenseFlags licenseFlags)
			{
				return true; // Accepts any valid license (even guest licenses)
			}

			protected override RegistrationData GetRegistrationData()
			{
				return new RegistrationData { Name = "Anonymous HearThisUser" };
			}

			protected override void HandleDeletedRegistration()
			{
				throw new NotImplementedException();
			}

			protected override void HandleChangedRegistrationData(RegistrationData registrationData)
			{
			}
		}
		#endregion

		public static void UpdateUiLanguageForUser(string languageId)
		{
			s_userInfo.UILanguageCode = languageId;
			Analytics.IdentifyUpdate(s_userInfo);
		}
	}
}
