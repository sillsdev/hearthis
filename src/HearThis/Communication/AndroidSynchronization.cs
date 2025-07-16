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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HearThis.Communication
{
	/// <summary>
	/// This class encapsulates the logic around performing synchronization with Android devices.
	/// </summary>
	public static class AndroidSynchronization
	{
		private const string kHearThisAndroidProductName = "HearThis Android";

		// Layout of a row in the IPv4 routing table.
		[StructLayout(LayoutKind.Sequential)]
		public struct MIB_IPFORWARDROW
		{
			public uint dwForwardDest;
			public uint dwForwardMask;
			public uint dwForwardPolicy;
			public uint dwForwardNextHop;
			public int dwForwardIfIndex;
			public int dwForwardType;
			public int dwForwardProto;
			public int dwForwardAge;
			public int dwForwardNextHopAS;
			public int dwForwardMetric1;  // the "interface metric" we need
			public int dwForwardMetric2;
			public int dwForwardMetric3;
			public int dwForwardMetric4;
			public int dwForwardMetric5;
		}

		// Hold a copy of the IPv4 routing table, which we will examine
		// to find which row/route has the lowest "interface metric".
		private static IntPtr RoutingTableBuf;

		// Layout of the routing table.
		[StructLayout(LayoutKind.Sequential)]
		private struct MIB_IPFORWARDTABLE
		{
			private int dwNumEntries;
			private MIB_IPFORWARDROW table;
		}

		// We use an unmanaged function in the C/C++ DLL "iphlpapi.dll".
		//   - "true": calling this function *can* set an error code,
		//     which will be retrieveable via Marshal.GetLastWin32Error()
		[DllImport("iphlpapi.dll", SetLastError = true)]
		static extern int GetIpForwardTable(IntPtr pIpForwardTable, ref int pdwSize, bool bOrder);

		// Hold relevant network interface attributes.
		private class InterfaceInfo
		{
			public string IpAddr      { get; set; }
			public string Description {	get; set; }
			public int Metric         { get; set; }
		}

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

			// To get our IP address we first need a copy of network stack's routing table.
			int tableSize = GetRoutingTable();
			if (tableSize == 0)
			{
				Debug.WriteLine("AndroidSynchronization, couldn't get routing table");
				return;
			}

			// Get our local IP address, which we will advertise.
			string localIp = GetInterfaceStackWillUse(parent);

			// Whether or not we got our IP address ok, we're done with copy of routing table so
			// release its buffer.
			Marshal.FreeHGlobal(RoutingTableBuf);

			if (localIp == "")
			{
				Debug.WriteLine("AndroidSynchronization, local IP not found");
				return;
			}

			var dlg = new AndroidSyncDialog();

			dlg.SetOurIpAddress(localIp);
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

		// Survey the network interfaces and determine which one, if any, the network stack
		// will use for network traffic.
		//   - During the assessment the current leading WiFi candidate will be held in
		//     'wifiInterface' and similarly the current best candidate for Ethernet will be
		//     in 'ethernetInterface'.
		//   - After assessment return the winner's IPv4 address, or an empty string if there
		//     is no winner.
		//
		private static string GetInterfaceStackWillUse(Form parent)
		{
			// Hold current network interface candidates, one each for WiFi and Ethernet.
			InterfaceInfo wifiInterface = new InterfaceInfo();
			InterfaceInfo ethernetInterface = new InterfaceInfo();

			// Initialize result structs metric field to the highest possible value
			// so the first real interface metric value seen will always replace it.
			wifiInterface.Metric = int.MaxValue;
			ethernetInterface.Metric = int.MaxValue;

			// Retrieve all network interfaces that are *active*.
			var allOperationalNetworks = NetworkInterface.GetAllNetworkInterfaces()
				.Where(ni => ni.OperationalStatus == OperationalStatus.Up).ToArray();

			if (!allOperationalNetworks.Any())
			{
				MessageBox.Show(parent, LocalizationManager.GetString("AndroidSynchronization.NetworkingRequired",
					"Android synchronization requires your computer to have networking enabled."),
					Program.kProduct);
				return "";
			}

			int currentIfaceMetric;

			// Get key attributes of active network interfaces.
			foreach (NetworkInterface ni in allOperationalNetworks)
			{
				// If we can't get IP or IPv4 properties for this interface, skip it.
				var ipProps = ni.GetIPProperties();
				if (ipProps == null)
				{
					continue;
				}
				var ipv4Props = ipProps.GetIPv4Properties();
				if (ipv4Props == null)
				{
					continue;
				}

				foreach (UnicastIPAddressInformation ip in ipProps.UnicastAddresses)
				{
					// We don't consider IPv6 so filter for IPv4 ('InterNetwork')...
					if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
					{
						// ...And of these we care only about WiFi and Ethernet.
						if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
						{
							currentIfaceMetric = GetMetricForInterface(ipv4Props.Index);

							// Save this interface if its metric is lowest we've seen so far.
							if (currentIfaceMetric < wifiInterface.Metric)
							{
								wifiInterface.IpAddr = ip.Address.ToString();
								wifiInterface.Description = ni.Description;
								wifiInterface.Metric = currentIfaceMetric;
							}
						}
						else if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
						{
							currentIfaceMetric = GetMetricForInterface(ipv4Props.Index);

							// Save this interface if its metric is lowest we've seen so far.
							if (currentIfaceMetric < ethernetInterface.Metric)
							{
								ethernetInterface.IpAddr = ip.Address.ToString();
								ethernetInterface.Description = ni.Description;
								ethernetInterface.Metric = currentIfaceMetric;
							}
						}
					}
				}
			}

			// Active network interfaces have all been assessed.
			//   - The WiFi interface having the lowest metric has been saved in the
			//     WiFi result struct. Note: if no active WiFi interface was seen then
			//     the result struct's metric field will still have its initial value.
			//   - Likewise for Ethernet.
			// Now choose the winner, if there is one:
			//   - If we saw an active WiFi interface, return its IP address
			//   - Else if we saw an active Ethernet interface, return its IP address
			//   - Else there is no winner so return an empty string

			if (wifiInterface.Metric < int.MaxValue)
			{
				Logger.WriteEvent($"Found a network for Android synchronization: {wifiInterface.Description}");
				Debug.WriteLine($"AndroidSynchronization, using Wi-Fi, local IP = {wifiInterface.IpAddr} ({wifiInterface.Description})");
				return wifiInterface.IpAddr;
			}
			if (ethernetInterface.Metric < int.MaxValue)
			{
				Logger.WriteEvent($"Found a network for Android synchronization: {ethernetInterface.Description}");
				Debug.WriteLine($"AndroidSynchronization, using Ethernet, local IP = {ethernetInterface.IpAddr} ({ethernetInterface.Description})");
				return ethernetInterface.IpAddr;
			}

			MessageBox.Show(parent, LocalizationManager.GetString("AndroidSynchronization.NoInterNetworkIPAddress",
				"Sorry, your network adapter has no InterNetwork IP address. If you do not know how to resolve this, please seek technical help.",
				Program.kProduct));
			Debug.WriteLine("AndroidSynchronization, local IP not found");
			return "";
		}

		// Get a copy of the network stack's routing table.
		// Put it in a buffer outside of this function so it can be examined multiple times
		// by other code.
		// Return: - buffer's length upon success
		//         - zero upon error
		//
		private static int GetRoutingTable()
		{
			// Preliminary: call with a buffer length of 0 (IntPtr.Zero) to learn how large
			// a buffer is needed to hold a copy of the routing table. The table is > 0, of
			// course, so GetIpForwardTable() fails with err 122 (ERROR_INSUFFICIENT_BUFFER).
			// We want the side effect: 'size' now > 0, filled in with the needed buffer length.
			int size = 0;
			int result = GetIpForwardTable(IntPtr.Zero, ref size, false);

			if (size == 0)
			{
				Debug.WriteLine("GetRoutingTable, ERROR getting buffer length: " + result);
				return 0;
			}

			// Allocate buffer for copy of routing table.
			// Will free it after conclusion of IP address determination logic.
			try
			{
				RoutingTableBuf = Marshal.AllocHGlobal(size);
			}
			catch (OutOfMemoryException e)
			{
				Debug.WriteLine("GetRoutingTable, ERROR creating buffer: " + e);
				return 0;
			}

			// Copy routing table into buffer. Will be examined elsewhere.
			result = GetIpForwardTable(RoutingTableBuf, ref size, false);
			if (result != 0)
			{
				// No table for us, something went wrong. Free the buffer.
				Debug.WriteLine("GetRoutingTable, ERROR getting table: " + result);
				Marshal.FreeHGlobal(RoutingTableBuf);
				return 0;
			}

			// All is well.
			return size;
		}

		// Get a key piece of info ("metric") from the specified network interface.
		// https://learn.microsoft.com/en-us/windows/win32/api/iphlpapi/nf-iphlpapi-getipforwardtable
		//
		// Retrieving the metric is not as simple as grabbing one of the fields in
		// the network interface. The metric resides in the network stack routing
		// table. One of the interface fields ("Index") is also in the routing table
		// and is how we correlate the two.
		//   - Calling code (walking the interface collection) passes in the index
		//     of the interface whose "best" metric it wants.
		//   - This function walks the routing table looking for all rows (each of
		//     which is a route) containing that index. It notes the metric in each
		//     route and returns the lowest among all routes/rows for the interface.
		//
		private static int GetMetricForInterface(int interfaceIndex)
		{
			int numEntries = 0;

			// Initialize to "worst" possible metric (Win10 Pro: 2^31 - 1).
			// It can only get better from there!
			int bestMetric = int.MaxValue;

			try
			{
				// Prepare for routing table survey: note how many entries it has.
				numEntries = Marshal.ReadInt32(RoutingTableBuf);

				// Advance pointer past the integer to point at 1st row.
				IntPtr rowPtr = IntPtr.Add(RoutingTableBuf, 4);

				// Walk the routing table looking for rows involving the the network
				// interface passed in. For each such row/route, check the metric.
				// If it is lower than the lowest we've yet seen, save it to become
				// the new benchmark.
				for (int i = 0; i < numEntries; i++)
				{
					MIB_IPFORWARDROW row = Marshal.PtrToStructure<MIB_IPFORWARDROW>(rowPtr);
					if (row.dwForwardIfIndex == interfaceIndex)
					{
						bestMetric = Math.Min(bestMetric, row.dwForwardMetric1);
					}
					rowPtr = IntPtr.Add(rowPtr, Marshal.SizeOf<MIB_IPFORWARDROW>());
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("GetMetricForInterface, ERROR, exception = " + e);
			}

			return bestMetric;
		}
	}
}
