using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using HearThis.Script;
using HearThis.UI;

namespace HearThis.Communication
{
	/// <summary>
	/// This class encapsulates the logic around performing synchronization with Android devices.
	/// </summary>
	public class AndroidSynchronization
	{
		public static void DoAndroidSync(Project project)
		{
			if (!project.IsRealProject)
			{
				MessageBox.Show("HearThis Android does not yet work properly with the Sample project. Please try a real one.",
					"Sorry");
				return;
			}
			var dlg = new AndroidSyncDialog();
			var network =
				NetworkInterface.GetAllNetworkInterfaces()
					.FirstOrDefault(
						x => x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && x.OperationalStatus == OperationalStatus.Up);
			if (network == null)
			{
				MessageBox.Show("Sync requires your computer to have wireless networking enabled");
				return;
			}
			var address =
				network.GetIPProperties()
					.UnicastAddresses.Where(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork)
					.FirstOrDefault();
			if (address == null)
			{
				MessageBox.Show("Your network adapter has no InterNetwork IP address. You will need technical help. Sorry.");
				return;
			}
			dlg.SetOurIpAddress(address.Address.ToString());
			dlg.ShowAndroidIpAddress(); // AFTER we set our IP address, which may be used to provide a default
			dlg.GotSync += (o, args) =>
			{
				var theirLink = new AndroidLink();
				// Enhance: some way to validate that we really got an IP address.
				theirLink.AndroidAddress = AndroidSyncDialog.AndroidIpAddress;
				var ourLink = new WindowsLink(Program.ApplicationDataBaseFolder);
				var merger = new RepoMerger(project, ourLink, theirLink);
				merger.Merge(dlg.ProgressBox);
				//Update info.txt on Android
				var projectInfoFilePath = project.GetProjectInfoFilePath();
				File.WriteAllText(projectInfoFilePath, project.GetProjectInfoFileContent());
				var theirInfoTxtPath = project.Name + "/" + Project.InfoTxtFileName;
				theirLink.PutFile(theirInfoTxtPath, File.ReadAllBytes(projectInfoFilePath));
				theirLink.SendNotification("syncCompleted");
				dlg.ProgressBox.WriteMessage("Sync completed successfully");
				//dlg.Close();
			};
			dlg.Show();
		}
	}
}
