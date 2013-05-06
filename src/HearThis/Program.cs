using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using HearThis.Properties;
using HearThis.Publishing;
using HearThis.UI;
using L10NSharp;
using NetSparkle;
using Palaso.IO;
using Palaso.Reporting;
using Paratext;
using Segmentio;
using Segmentio.Model;

namespace HearThis
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			//bring in settings from any previous version
			if (Settings.Default.NeedUpgrade)
			{
				//see http://stackoverflow.com/questions/3498561/net-applicationsettingsbase-should-i-call-upgrade-every-time-i-load
				Settings.Default.Upgrade();
				Settings.Default.NeedUpgrade = false;
				Settings.Default.Save();
			}

			SetUpErrorHandling();
			SetUpReporting();
			SetupLocalization();

			if (args.Length == 1 && args[0].Trim() == "-afterInstall")
			{
				using (var dlg = new Palaso.UI.WindowsForms.ReleaseNotes.ShowReleaseNotesDialog(Resources.HearThis,  FileLocator.GetFileDistributedWithApplication( "releaseNotes.md")))
				{
					dlg.ShowDialog();
				}
			}

			if (Control.ModifierKeys == Keys.Control)
			{
				Settings.Default.Project = "Sample";
			}
			else
			{

				try
				{
					ScrTextCollection.Initialize();
				}
				catch (Exception error)
				{
					ErrorReport.NotifyUserOfProblem(
						LocalizationManager.GetString("Program.ParatextNotInstalled",
							"It looks like perhaps Paratext is not installed on this computer, or there is some other problem connecting to it. We'll set you up with a sample so you can play with HearThis, but you'll have to install Paratext to get any real work done here.",
							""));

					Settings.Default.Project = "Sample";
				}
			}

			Application.Run(new Shell());
			Analytics.Client.Dispose();
		}

		private static void SetupLocalization()
		{
			var installedStringFileFolder = FileLocator.GetDirectoryDistributedWithApplication("localization");
			// Enhance JohnT: LineRecordingRepository is probably not the best place for this shared functionality?
			// Review JohnT: is this the right place for it? Puts Localizations in among the project folders for recordings.
			// But somewhere in the application data folder for this application feels like the right place.
			var targetTmxFilePath = LineRecordingRepository.GetApplicationDataFolder("Localizations");
			string desiredUiLangId = Settings.Default.UserInterfaceLanguage;
			LocalizationManager.Create(desiredUiLangId, "HearThis", Application.ProductName,
									   Application.ProductVersion, installedStringFileFolder,
									   targetTmxFilePath, Resources.HearThis, IssuesEmailAddress, "HearThis");
			// Set up localization for Palaso UI components etc.
			// Review: should we be using HearThis's product version here? If not what?
			LocalizationManager.Create(desiredUiLangId, "Palaso", "Palaso", Application.ProductVersion, installedStringFileFolder,
									   targetTmxFilePath, Resources.HearThis, IssuesEmailAddress, "Palaso.UI");

		}

		/// <summary>
		/// The email address people should write to with problems (or new localizations?) for HearThis.
		/// Todo: is this the right address? need to create an account and have someone monitor it.
		/// </summary>
		public static string IssuesEmailAddress
		{
			get { return "issues@hearthis.palaso.org"; }
		}

		static void Client_Succeeded(BaseAction action)
		{
			Debug.WriteLine("SegmentIO succeeded: "+action.GetAction());
		}

		static void Client_Failed(BaseAction action, Exception e)
		{
			Debug.WriteLine("**** Segment.IO Failed to deliver");
		}


		/// ------------------------------------------------------------------------------------
		private static void SetUpErrorHandling()
		{
			ErrorReport.EmailAddress = "replace@gmail.com".Replace("replace", "hattonjohn");
			ErrorReport.AddStandardProperties();
			ExceptionHandler.Init();
			ExceptionHandler.AddDelegate(ReportError);
		}

		private static void ReportError(object sender, CancelExceptionHandlingEventArgs e)
		{
			Analytics.Client.Track(Settings.Default.IdForAnalytics, "Got Error Report", new Segmentio.Model.Properties() {
				{ "Message", e.Exception.Message },
				{ "Stack Trace", e.Exception.StackTrace }
				});
		}

		/// ------------------------------------------------------------------------------------
		private static void SetUpReporting()
		{
			//we're not using this yet, it's from the old palaso one, but something like this might be useful
			/*if (Settings.Default.Reporting == null)
			{
				Settings.Default.Reporting = new ReportingSettings();
				Settings.Default.Save();
			}*/

#if DEBUG

			Analytics.Initialize("pldi6z3n3vfz23czhano"); //https://segment.io/hearthisdebug
			Analytics.Client.Failed += Client_Failed;
			Analytics.Client.Succeeded += Client_Succeeded;
#else
			Analytics.Initialize("bh7aaqmlmd0bhd48g3ye"); //https://segment.io/hearthis
#endif
			if (string.IsNullOrEmpty(Settings.Default.IdForAnalytics))
			{
				Settings.Default.IdForAnalytics = Guid.NewGuid().ToString();
				Settings.Default.Save();
			}

			Analytics.Client.Identify(Settings.Default.IdForAnalytics, new Traits()
				{
					//{ "Name", "joe shmo" },
					//{ "Email", "joshmo@example.com" },
				});
		}
	}
}
