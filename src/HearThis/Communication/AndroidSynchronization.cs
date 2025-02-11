// --------------------------------------------------------------------------------------------
#region // Copyright (c) 2016-2025, SIL Global.
// <copyright from='2016' to='2025' company='SIL Global'>
//		Copyright (c) 2016-2025, SIL Global.
//
//		Distributable under the terms of the MIT License (https://sil.mit-license.org/)
// </copyright>
#endregion
// --------------------------------------------------------------------------------------------
using System;
using HearThis.Script;
using HearThis.UI;
using SIL.IO;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;
using DesktopAnalytics;
using L10NSharp;
using SIL.Reporting;
using static System.String;

namespace HearThis.Communication
{
	/// <summary>
	/// This class encapsulates the logic around performing synchronization with Android devices.
	/// </summary>
	public static class AndroidSynchronization
	{
		private const string kHearThisAndroidProductName = "HearThis Android";
		public static void DoAndroidSync(Project project, Form parent)
		{
			if (!project.IsRealProject)
			{
				MessageBox.Show(parent, Format(
					LocalizationManager.GetString("AndroidSynchronization.DoNotUseSampleProject",
					"Sorry, {0} does not yet work properly with the Sample project. Please try a real one.",
					"Param 0: \"HearThis Android\" (product name)"), kHearThisAndroidProductName),
					Program.kProduct);
				return;
			}
			var dlg = new AndroidSyncDialog();
			var address = GetNetworkAddress(parent);

			if (address == null)
				return;

			dlg.SetOurIpAddress(address.ToString());
			dlg.ShowAndroidIpAddress(); // AFTER we set our IP address, which may be used to provide a default
			dlg.GotSync += (o, args) =>
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
					var merger = new RepoMerger(project, ourLink, theirLink);
					merger.Merge(project.StylesToSkipByDefault, dlg.ProgressBox);
					//Update info.txt on Android
					var infoFilePath = project.GetProjectRecordingStatusInfoFilePath();
					RobustFile.WriteAllText(infoFilePath, project.GetProjectRecordingStatusInfoFileContent());
					var theirInfoTxtPath = project.Name + "/" + Project.InfoTxtFileName;
					theirLink.PutFile(theirInfoTxtPath, File.ReadAllBytes(infoFilePath));
					theirLink.SendNotification("syncCompleted");
					dlg.ProgressBox.WriteMessage("Sync completed successfully");
					//dlg.Close();
				}
				catch (WebException ex)
				{
					string msg;
					switch (ex.Status)
					{
						case WebExceptionStatus.NameResolutionFailure:
							msg = LocalizationManager.GetString(
								"AndroidSynchronization.NameResolutionFailure",
								"HearThis could not make sense of the address you gave for the " +
								"device. Please try again.");
							break;
						case WebExceptionStatus.ConnectFailure:
							msg = LocalizationManager.GetString(
								"AndroidSynchronization.ConnectFailure",
								"HearThis could not connect to the device. Check to be sure the " +
								"devices are on the same WiFi network and that there is not a " +
								"firewall blocking things.");
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
										"Please try again, or report the problem if it keeps happening"),
									ex.Message);

							break;
						}
					}

					dlg.ProgressBox.WriteError(msg);
				}
			};
			dlg.Show(parent);
		}

		private static IPAddress GetNetworkAddress(Form parent)
		{
			// Retrieve all network interfaces
			var allOperationalNetworks = NetworkInterface.GetAllNetworkInterfaces()
				.Where(ni => ni.OperationalStatus == OperationalStatus.Up).ToArray();

			if (!allOperationalNetworks.Any())
			{
				MessageBox.Show(parent, LocalizationManager.GetString("AndroidSynchronization.NetworkingRequired",
					"Android synchronization requires your computer to have networking enabled."),
					Program.kProduct);
				return null;
			}

			(NetworkInterface Network, IPAddress Address)? preferred = null;

			foreach (var network in allOperationalNetworks)
			{
				var ipAddress = network.GetIPProperties().UnicastAddresses.Select(ip => ip.Address)
					.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

				if (ipAddress != null)
				{
					if (network.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
					{
						preferred = (network, ipAddress);
						break;
					}

					if (preferred == null)
						preferred = (network, ipAddress);
				}
			}

			if (!preferred.HasValue)
			{
				MessageBox.Show(parent, LocalizationManager.GetString("AndroidSynchronization.NoInterNetworkIPAddress",
					"Sorry, your network adapter has no InterNetwork IP address. If you do not know how to resolve this, please seek technical help.",
					Program.kProduct));
				return null;
			}

			Logger.WriteEvent("Found " +
				$"{(preferred.Value.Network.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ? "a" : "only a wired")}" +
				$" network for Android synchronization: {preferred.Value.Network.Description}.");
			
			return preferred.Value.Address;
		}
	}
}
