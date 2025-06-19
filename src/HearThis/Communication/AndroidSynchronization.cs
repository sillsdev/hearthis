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
using NDesk.DBus;

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

		// Hold the current network interface candidates, one for Wi-Fi and one
		// for Ethernet.
		static InterfaceInfo IfaceWifi = new InterfaceInfo();
		static InterfaceInfo IfaceEthernet = new InterfaceInfo();

		// Possible results from network interface assessment.
		private enum CommTypeToExpect
		{
			None = 0,
			WiFi = 1,
			Ethernet = 2
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
			var dlg = new AndroidSyncDialog();

			// Examine the network interfaces and determine which will be used for network traffic.
			// Candidates will get stored in the two results objects.
			CommTypeToExpect ifcResult = GetInterfaceStackWillUse(parent);

			if (ifcResult == CommTypeToExpect.None)
			{
				Debug.WriteLine("WM, AndroidSynchronization, local IP not found");
				return;
			}

			string address = "";

			if (ifcResult == CommTypeToExpect.WiFi)
			{
				// Network stack will use WiFi.
				address = IfaceWifi.IpAddr;
				Debug.WriteLine("WM, AndroidSynchronization, local IP = {0} ({1})", IfaceWifi.IpAddr, IfaceWifi.Description);
			}
			else
			{
				// Network stack will use Ethernet.
				address = IfaceEthernet.IpAddr;
				Debug.WriteLine("WM, AndroidSynchronization, local IP = {0} ({1})", IfaceEthernet.IpAddr, IfaceEthernet.Description);
			}

			dlg.SetOurIpAddress(address);
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
		//     'IfaceWifi', and similarly the current best candidate for Ethernet will be
		//     in 'IfaceEthernet'.
		//   - After assessment inform calling code of the winner by returning an enum
		//     indicating which of the candidate structs to draw from: WiFi, Ethernet,
		//     or neither.
		//
		private static CommTypeToExpect GetInterfaceStackWillUse(Form parent)
		{
			int currentIfaceMetric;

			// Initialize result structs metric field to the highest possible value
			// so the first interface metric value seen will always replace it.
			IfaceWifi.Metric = int.MaxValue;
			IfaceEthernet.Metric = int.MaxValue;

			// Retrieve all network interfaces that are *active*.
			var allOperationalNetworks = NetworkInterface.GetAllNetworkInterfaces()
				.Where(ni => ni.OperationalStatus == OperationalStatus.Up).ToArray();

			if (!allOperationalNetworks.Any())
			{
				MessageBox.Show(parent, LocalizationManager.GetString("AndroidSynchronization.NetworkingRequired",
					"Android synchronization requires your computer to have networking enabled."),
					Program.kProduct);
				Debug.WriteLine("WM, AndroidSynchronization, no network interfaces are operational");
				return CommTypeToExpect.None;
			}

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

				Debug.WriteLine("WM, AndroidSynchronization, checking IP addresses in " + ni.Name);  // TEMPORARY
				foreach (UnicastIPAddressInformation ip in ipProps.UnicastAddresses)
				{
					// We don't consider IPv6 so filter for IPv4 ('InterNetwork')...
					if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
					{
						// ...And of these we care only about WiFi and Ethernet.
						if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
						{
							Debug.WriteLine("  WiFi...");  // TEMPORARY
							currentIfaceMetric = GetMetricForInterface(ipv4Props.Index);

							// Save this interface if its metric is lowest we've seen so far.
							if (currentIfaceMetric < IfaceWifi.Metric)
							{
								Debug.WriteLine("  updating WiFi metric to " + currentIfaceMetric);  // TEMPORARY
								IfaceWifi.IpAddr = ip.Address.ToString();
								IfaceWifi.Description = ni.Description;
								IfaceWifi.Metric = currentIfaceMetric;
							}
						}
						else if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
						{
							Debug.WriteLine("  Ethernet...");  // TEMPORARY
							currentIfaceMetric = GetMetricForInterface(ipv4Props.Index);

							// Save this interface if its metric is lowest we've seen so far.
							if (currentIfaceMetric < IfaceEthernet.Metric)
							{
								Debug.WriteLine("  updating Ethernet metric to " + currentIfaceMetric);  // TEMPORARY
								IfaceEthernet.IpAddr = ip.Address.ToString();
								IfaceEthernet.Description = ni.Description;
								IfaceEthernet.Metric = currentIfaceMetric;
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
			//   - If we saw an active WiFi interface, return that
			//   - Else if we saw an active Ethernet interface, return that
			//   - Else there is no winner so return none
			if (IfaceWifi.Metric < int.MaxValue)
			{
				Debug.WriteLine("WM, WiFi wins, interface = " + IfaceWifi.Description);  // TEMPORARY
				Logger.WriteEvent("Found " +
				$"a network for Android synchronization: " + IfaceWifi.Description);
				return CommTypeToExpect.WiFi;
			}
			if (IfaceEthernet.Metric < int.MaxValue)
			{
				Debug.WriteLine("WM, Ethernet wins, interface = " + IfaceEthernet.Description);  // TEMPORARY
				Logger.WriteEvent("Found " +
				$"a network for Android synchronization: " + IfaceEthernet.Description);
				return CommTypeToExpect.Ethernet;
			}

			MessageBox.Show(parent, LocalizationManager.GetString("AndroidSynchronization.NoInterNetworkIPAddress",
				"Sorry, your network adapter has no InterNetwork IP address. If you do not know how to resolve this, please seek technical help.",
				Program.kProduct));
			return CommTypeToExpect.None;
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
			// Initialize to "worst" possible metric (Win10 Pro: 2^31 - 1).
			// It can only get better from there!
			int bestMetric = int.MaxValue;

			// Preliminary: call with a null buffer ('size') to learn how large a
			// buffer is needed to hold a copy of the routing table.
			int size = 0;
			GetIpForwardTable(IntPtr.Zero, ref size, false);

			IntPtr tableBuf;

			try
			{
				// 'size' now shows how large a buffer is needed, so allocate it.
				tableBuf = Marshal.AllocHGlobal(size);
			}
			catch (OutOfMemoryException e)
			{
				Debug.WriteLine("  GetMetricForInterface, ERROR creating buffer: " + e);
				return bestMetric;
			}

			try
			{
				// Copy the routing table into buffer for examination.
				int error = GetIpForwardTable(tableBuf, ref size, false);
				if (error != 0)
				{
					// Something went wrong so bail.
					// It is tempting to add a dealloc call here, but don't. The
					// dealloc in the 'finally' block *will* be done (I checked).
					Console.WriteLine("  GetMetricForInterface, ERROR, GetIpForwardTable() = {0}, returning {1}", error, bestMetric);
					return bestMetric;
				}

				// Get number of routing table entries.
				int numEntries = Marshal.ReadInt32(tableBuf);

				// Advance pointer past the integer to point at 1st row.
				IntPtr rowPtr = IntPtr.Add(tableBuf, 4);

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
				if (e is AccessViolationException || e is MissingMethodException)
				{
					Debug.WriteLine("  GetMetricForInterface, ERROR: " + e);
				}
			}
			finally
			{
				Marshal.FreeHGlobal(tableBuf);
			}
			return bestMetric;
		}
	}
}
