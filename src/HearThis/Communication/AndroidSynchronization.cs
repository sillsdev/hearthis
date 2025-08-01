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
using SIL.EventsAndDelegates;
using SIL.IO;
using SIL.Reporting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.String;

namespace HearThis.Communication
{
	/// <summary>
	/// This class encapsulates the logic around performing synchronization with Android devices.
	/// </summary>
	public static class AndroidSynchronization
	{
		private const string kHearThisAndroidProductName = "HearThis Android";
		private const int ERROR_INSUFFICIENT_BUFFER = 122;  // from winerror.h

		// Layout of a row in the IPv4 routing table.
		[StructLayout(LayoutKind.Sequential)]
		private struct MIB_IPFORWARDROW
		{
			public uint dwForwardDest;
			public uint dwForwardMask;
			public uint dwForwardPolicy;
			public uint dwForwardNextHop;
			public uint dwForwardIfIndex;
			public int dwForwardType;
			public int dwForwardProto;
			public int dwForwardAge;
			public uint dwForwardNextHopAS;
			public int dwForwardMetric1;  // the "interface metric" we need
			public int dwForwardMetric2;
			public int dwForwardMetric3;
			public int dwForwardMetric4;
			public int dwForwardMetric5;
		}

		// Layout of the routing table.
		[StructLayout(LayoutKind.Sequential)]
		private struct MIB_IPFORWARDTABLE
		{
			public int dwNumEntries;
			public MIB_IPFORWARDROW route;
		}

		// We use an unmanaged function in the C/C++ DLL "iphlpapi.dll".
		//   - "true": calling this function *can* set an error code,
		//     which will be retrieveable via Marshal.GetLastWin32Error()
		[DllImport("iphlpapi.dll", SetLastError = true)]
		private static extern int GetIpForwardTable(IntPtr pIpForwardTable, ref int pdwSize, bool bOrder);

		// Hold relevant network interface attributes.
		private class InterfaceInfo
		{
			public string IpAddr      { get; set; }
			public string Description { get; set; }
			public int Metric         { get; set; }
		}

		// This class creates and owns a copy of the routing table. The copy only
		// lives long enough to have its route metrics copied into a lookup table,
		// which supports queries from elsewhere.
		//
		public sealed class RoutingTableProxy
		{
			private bool Ready = false;
			private IntPtr RoutingTableBuf = IntPtr.Zero;

			// Lookup table containing the *lowest* interface metric value for each
			// network interface. Data is pulled, selectively, from the routing table.
			private readonly Dictionary<uint, int> _metricsByInterfaceIndex;

			public RoutingTableProxy()
			{
				_metricsByInterfaceIndex = new Dictionary<uint, int>();
				LoadRoutingTable();
			}

			public bool IsValid()
			{
				return Ready;
			}

			// Find the given interface in the lookup table and return its metric.
			// While this table was being populated, care was taken to ensure that
			// it has the lowest value for each interface.
			// 
			public int GetMetricForInterface(uint interfaceIndex)
			{
				int metric;
				_metricsByInterfaceIndex.TryGetValue(interfaceIndex, out metric);
				return metric;
			}

			// This method owns the buffer holding a copy of the routing table.
			// The entire buffer lifecycle is here: create it, fill it with routing table
			// data, parse it into a lookup table, and finally free it.
			//
			private void LoadRoutingTable()
			{
				int size = 0;

				try
				{
					// First call to get the size. We expect an error, and 'size'
					// will then contain the value of needed buffer length.
					int result = GetIpForwardTable(IntPtr.Zero, ref size, true);
					if (result != ERROR_INSUFFICIENT_BUFFER)
					{
						Debug.WriteLine($"AndroidSynchronization, error ({result}) getting size");
						throw new Win32Exception(result);
					}

					RoutingTableBuf = Marshal.AllocHGlobal(size);

					// Second call gets the table.
					result = GetIpForwardTable(RoutingTableBuf, ref size, true);
					if (result != 0)
					{
						Debug.WriteLine($"AndroidSynchronization, error ({result}) getting buffer or table");
						throw new Win32Exception(result);
					}

					// Parse the buffer: for each row (which is a route) get the interface
					// metric. Store it plus its index into a lookup table ONLY IF either:
					//   1. lookup table does not yet contain this index, or
					//   2. lookup table already has this index but the associated metric
					//      value is higher and we want to now overwrite it

					int metric;
					var table = Marshal.PtrToStructure<MIB_IPFORWARDTABLE>(RoutingTableBuf);
					IntPtr rowPtr = IntPtr.Add(RoutingTableBuf, Marshal.OffsetOf<MIB_IPFORWARDTABLE>("route").ToInt32());

					for (int i = 0; i < table.dwNumEntries; i++)
					{
						MIB_IPFORWARDROW row = Marshal.PtrToStructure<MIB_IPFORWARDROW>(rowPtr);

						if (_metricsByInterfaceIndex.TryGetValue(row.dwForwardIfIndex, out metric) == false)
						{
							// meets criterion #1
							_metricsByInterfaceIndex[row.dwForwardIfIndex] = row.dwForwardMetric1;
						}
						else if (row.dwForwardMetric1 < metric)
						{
							// meets criterion #2
							_metricsByInterfaceIndex[row.dwForwardIfIndex] = row.dwForwardMetric1;
						}
						rowPtr = IntPtr.Add(rowPtr, Marshal.SizeOf<MIB_IPFORWARDROW>());
					}

					// Got through entire buffer without exception so say we're good.
					Ready = true;
				}
				catch (Exception e)
				{
					Debug.WriteLine($"AndroidSynchronization, got exception ({e})");
				}
				finally
				{
					if (RoutingTableBuf != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(RoutingTableBuf);
					}
				}
			}
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

			// To set up for getting the local IP address, get routing table data.
			RoutingTableProxy proxy = new RoutingTableProxy();

			if (!proxy.IsValid())
			{
				Debug.WriteLine("AndroidSynchronization, error instantiating proxy");
				return;
			}

			// Get our local IP address, which we will advertise.
			string localIp = GetInterfaceStackWillUse(parent, proxy);

			if (localIp == null)
			{
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
										"Please try again, or report the problem if it keeps happening."),
									ex.Message);

							break;
						}
					}
					dlg.ProgressBox.WriteError(msg);
				}
			};
			dlg.Show(parent);
		}

		// Survey the network interfaces, determine which one (if any) the network stack
		// will use for network traffic, and return the appropriate IP address.
		//   - During the assessment the current leading WiFi candidate will be held in
		//     'wifiInterface' and similarly the current best candidate for Ethernet will be
		//     in 'ethernetInterface'.
		//   - After assessment return the winner's IPv4 address, or null if there
		//     is no winner.
		//
		private static string GetInterfaceStackWillUse(Form parent, RoutingTableProxy proxy)
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

				uint uIndex = (uint)ipv4Props.Index;

				foreach (UnicastIPAddressInformation ip in ipProps.UnicastAddresses)
				{
					// We don't consider IPv6 so filter for IPv4 ('InterNetwork')...
					if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
					{
						// ...And of these we care only about WiFi and Ethernet.
						if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
						{
							currentIfaceMetric = proxy.GetMetricForInterface(uIndex);

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
							currentIfaceMetric = proxy.GetMetricForInterface(uIndex);

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
				Debug.WriteLine($"AndroidSynchronization using Wi-Fi, local IP = {wifiInterface.IpAddr} ({wifiInterface.Description})");
				return wifiInterface.IpAddr;
			}
			if (ethernetInterface.Metric < int.MaxValue)
			{
				Logger.WriteEvent($"Found a network for Android synchronization: {ethernetInterface.Description}");
				Debug.WriteLine($"AndroidSynchronization using Ethernet, local IP = {ethernetInterface.IpAddr} ({ethernetInterface.Description})");
				return ethernetInterface.IpAddr;
			}

			MessageBox.Show(parent, LocalizationManager.GetString(
				"AndroidSynchronization.NoInterNetworkIPAddress",
				"Sorry, your network adapter does not have a valid IP address for " +
				"connecting to other networks. If you are not sure how to fix this, " +
				"please seek technical help."),
				Program.kProduct);
			Debug.WriteLine("AndroidSynchronization, local IP address not found");
			return null;
		}
	}
}
