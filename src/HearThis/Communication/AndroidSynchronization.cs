using HearThis.Script;
using HearThis.UI;
using SIL.IO;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;
using L10NSharp;
using static System.String;

namespace HearThis.Communication
{
	/// <summary>
	/// This class encapsulates the logic around performing synchronization with Android devices.
	/// </summary>
	public static class AndroidSynchronization
	{
		private const string kHearThisAndroidProductName = "HearThis Android";
		public static void DoAndroidSync(Project project)
		{
			if (!project.IsRealProject)
			{
				MessageBox.Show(Format(
					LocalizationManager.GetString("AndroidSynchronization.DoNotUseSampleProject",
					"Sorry, {0} does not yet work properly with the Sample project. Please try a real one.",
					"Param is \"HearThis Android\" (product name)")),
					Program.kProduct);
				return;
			}
			var dlg = new AndroidSyncDialog();
			var network =
				NetworkInterface.GetAllNetworkInterfaces()
					.FirstOrDefault(
						x => x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && x.OperationalStatus == OperationalStatus.Up);
			if (network == null)
			{
				MessageBox.Show(LocalizationManager.GetString("AndroidSynchronization.WirelessNetworkingRequired",
					"Sync requires your computer to have wireless networking enabled"),
					Program.kProduct);
				return;
			}
			var address = network.GetIPProperties()
				.UnicastAddresses.FirstOrDefault(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork);
			if (address == null)
			{
				MessageBox.Show(LocalizationManager.GetString("AndroidSynchronization.NoInterNetworkIPAddress",
					"Sorry, your network adapter has no InterNetwork IP address. If you do not know how to resolve this, please seek technical help.",
					Program.kProduct));
				return;
			}
			dlg.SetOurIpAddress(address.Address.ToString());
			dlg.ShowAndroidIpAddress(); // AFTER we set our IP address, which may be used to provide a default
			dlg.GotSync += (o, args) =>
			{
				try
				{
					var theirLink = new AndroidLink();
					theirLink.AndroidAddress = AndroidSyncDialog.AndroidIpAddress;
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
					if (ex.Status == WebExceptionStatus.NameResolutionFailure)
					{
						dlg.ProgressBox.WriteError("HearThis could not make sense of the address you gave for the device. Please try again.");
					} else if (ex.Status == WebExceptionStatus.ConnectFailure)
					{
						dlg.ProgressBox.WriteError("HearThis could not connect to the device. Check to be sure the devices are on the same WiFi network and that there is not a firewall blocking things.");
					} else if (ex.Status == WebExceptionStatus.ConnectionClosed)
					{
						dlg.ProgressBox.WriteError("The connection to the device closed unexpectedly. Please don't try to use the device for other things during the transfer. If the device is going to sleep, you can change settings to prevent this.");
					}
					else
					{
						dlg.ProgressBox.WriteError("Something went wrong with the transfer. The system message is {0}. Please try again, or report the problem if it keeps happening", ex.Message);
					}
				}
			};
			dlg.Show();
		}
	}
}
