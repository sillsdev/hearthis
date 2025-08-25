// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2016-2025, SIL Global.
// <copyright from='2016' to='2025' company='SIL Global'>
//		Copyright (c) 2016-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using DesktopAnalytics;
using HearThis.Script;
using HearThis.UI;
using L10NSharp;
using SIL.IO;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HearThis.Communication.PreferredNetworkInterfaceResolver;
using static System.String;

namespace HearThis.Communication
{
	/// <summary>
	/// This class encapsulates the logic around performing synchronization with Android devices.
	/// </summary>
	public static class AndroidSynchronization
	{
		public static void DoAndroidSync(Project project, Form parent)
		{
			if (!project.IsRealProject)
			{
				MessageBox.Show(parent, Format(
					LocalizationManager.GetString("AndroidSynchronization.DoNotUseSampleProject",
					"Sorry, {0} for Android does not yet work properly with the {1} project. Please try a real one.",
					"Param 0: \"HearThis\" (Android app name); Param 1: \"Sample\" (project name)"),
					Program.kAndroidAppName, SampleScriptProvider.kProjectUiName),
					Program.kProduct);
				return;
			}

			// Get our local IP address, which we will advertise.
			var resolver = new PreferredNetworkInterfaceResolver();
			var localIp = resolver.GetBestActiveInterface(out var failureReason);
			if (localIp == null)
			{
				string msg = null;

				switch (failureReason)
				{
					case FailureReason.NetworkingNotEnabled:
						msg = LocalizationManager.GetString(
							"AndroidSynchronization.NetworkingRequired",
							"Android synchronization requires your computer to have networking enabled.");
						break;
					case FailureReason.NoInterNetworkIPAddress:
						msg = LocalizationManager.GetString(
							"AndroidSynchronization.NoInterNetworkIPAddress",
							"Sorry, your network adapter does not have a valid IP address for " +
							"connecting to other networks. If you are not sure how to fix this, " +
							"please seek technical help.");
						break;
				}

				MessageBox.Show(parent, msg, Program.kProduct);
				return;
			}

			var dlg = new AndroidSyncDialog();

			dlg.SetOurIpAddress(localIp);
			dlg.ShowAndroidIpAddress(); // AFTER we set our IP address, which may be used to provide a default
			dlg.GotSync += async (o, args) =>
			{
				try
				{
					bool RetryOnTimeout(WebException ex, string path)
					{
						var response = MessageBox.Show(parent, ex.Message + Environment.NewLine +
							Format(LocalizationManager.GetString(
								"AndroidSynchronization.RetryOnTimeout",
								"Attempting to copy {0}\r\nChoose Abort to stop the sync (but not " +
								"roll back anything already synchronized).\r\nChoose Retry to " +
								"attempt to sync this file again with a longer timeout.\r\n" +
								"Choose Ignore to skip this file and keep the existing one on this " +
								"computer but continue attempting to sync any remaining files. ",
								"Param 0: file path on Android system"),
								path), Program.kProduct, MessageBoxButtons.AbortRetryIgnore);
						switch (response)
						{
							case DialogResult.Abort:
								throw ex;
							case DialogResult.Retry:
								return true;
							case DialogResult.Ignore:
								return false;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}

					Analytics.Track("Sync with Android");
					var theirLink = new AndroidLink(AndroidSyncDialog.AndroidIpAddress,
						RetryOnTimeout);
					var ourLink = new WindowsLink(Program.ApplicationDataBaseFolder);
					Debug.WriteLine("AndroidSynchronization, theirLink = " + theirLink);
					Debug.WriteLine("AndroidSynchronization, ourLink = " + ourLink);
					var merger = new RepoMerger(project, ourLink, theirLink);

					var progressMsgFmt = LocalizationManager.GetString(
						"AndroidSynchronization.Progress.MessageFormat",
						"Syncing {0}, chapter {1}",
						"Param 0: Scripture book name; Param 1: chapter number");

					// Run the merge off the UI thread
					var mergeCompleted = await Task.Run(() => merger.Merge(
						project.StylesToSkipByDefault, dlg.ProgressBox, progressMsgFmt));

					// REVIEW: Should we check for cancellation here and not even attempt to write
					// the project info file? That could be what's causing the Android app to end
					// up in a corrupt state following cancellation.
					//Update info.txt on Android
					var infoFilePath = project.GetProjectRecordingStatusInfoFilePath();
					Debug.WriteLine("AndroidSynchronization, infoFilePath = " + infoFilePath);
					RobustFile.WriteAllText(infoFilePath, project.GetProjectRecordingStatusInfoFileContent());
					var theirInfoTxtPath = project.Name + "/" + Project.InfoTxtFileName;
					Debug.WriteLine("AndroidSynchronization, theirInfoTxtPath = " + theirInfoTxtPath);
					theirLink.PutFile(theirInfoTxtPath, File.ReadAllBytes(infoFilePath));
					if (mergeCompleted)
					{
						Debug.WriteLine("AndroidSynchronization, mergeCompleted = TRUE, sending sync_success");
						theirLink.SendNotification("sync_success");
						dlg.ProgressBox.WriteMessage(LocalizationManager.GetString(
							"AndroidSynchronization.Progress.Completed",
							"Sync completed successfully"));
					}
					else
					{
						// TODO (HT-508): Send a specific notification so HTA knows the sync was
						// interrupted.
						Debug.WriteLine("AndroidSynchronization, mergeCompleted = FALSE, sending sync_interrupted");
						theirLink.SendNotification("sync_interrupted");
						dlg.ProgressBox.WriteMessage(LocalizationManager.GetString(
							"AndroidSynchronization.Progress.Canceled",
							"Sync was canceled by the user."));
					}
				}
				catch (WebException ex)
				{
					string msg;
					switch (ex.Status)
					{
						case WebExceptionStatus.NameResolutionFailure:
							msg = Format(LocalizationManager.GetString(
								"AndroidSynchronization.NameResolutionFailure",
								"{0} could not make sense of the address you gave for the " +
								"device. Please try again.",
								"Param is \"HearTHis\" (product name)") + Environment.NewLine +
								ex.Message,
								Program.kProduct);
							break;
						case WebExceptionStatus.ConnectFailure:
							msg = Format(LocalizationManager.GetString(
								"AndroidSynchronization.ConnectFailure",
								"{0} could not connect to the device. Check to be sure the " +
								"devices are on the same Wi-Fi network and that there is not a " +
								"firewall blocking things.",
								"Param is \"HearTHis\" (product name)"),
								Program.kProduct);
							if (ex.InnerException is SocketException socketEx)
								msg += Environment.NewLine + socketEx.Message;
							break;
						case WebExceptionStatus.ConnectionClosed:
							msg = LocalizationManager.GetString(
								"AndroidSynchronization.ConnectionClosed",
								"The connection to the device closed unexpectedly. Please don't " +
								"try to use the device for other things during the transfer. If the " +
								"device is going to sleep, you can change settings to prevent this.");
							break;
						default:
						{
							msg = ex.Response is HttpWebResponse response &&
								response.StatusCode == HttpStatusCode.RequestTimeout ? ex.Message :
									Format(LocalizationManager.GetString(
										"AndroidSynchronization.OtherWebException",
										"Something went wrong with the transfer. The system message is {0}. " +
										"Please try again, or report the problem if it keeps happening."),
									ex.Message);

							break;
						}
					}
					dlg.ProgressBox.WriteError(msg);
				}

				dlg.SyncInProgress = false;
			};
			dlg.ShowDialog(parent);
		}
	}
}
