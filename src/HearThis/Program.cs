using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using HearThis.Properties;
using HearThis.UI;
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
					Palaso.Reporting.ErrorReport.NotifyUserOfProblem(
						"It looks like perhaps Paratext is not installed on this computer, or there is some other problem connecting to it. We'll set you up with a sample so you can play with HearThis, but you'll have to install Paratext to get any real work done here.");

					Settings.Default.Project = "Sample";
				}
			}

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
				//{ "Name", "john hatton" },
				//{ "Email", "hattonjohn@gmail.com" },
			});
			Application.Run(new Shell());
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
			if (Settings.Default.Reporting == null)
			{
				Settings.Default.Reporting = new ReportingSettings();
				Settings.Default.Save();
			}

			UsageReporter.Init(Settings.Default.Reporting, "hearthis.palaso.org", "UA-22170471-7",
#if DEBUG
							   true
#else
				false
#endif
				);
		}
	}
}
