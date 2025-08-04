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

		// Layout of a row in the IPv4 routing table.
		[StructLayout(LayoutKind.Sequential)]
		private struct MIB_IPFORWARDROW
		{
			public uint Destination;
			public uint SubnetMask;
			public uint Policy;
			public uint NextHop;
			public uint InterfaceIndex;
			public int Type;
			public int Protocol;
			public int Age;
			public uint NextHopAS;
			public int RouteCost;  // dwForwardMetric1: the "interface metric" we care about
			// These metrics are part of the original structure spec but are not used in modern
			// routing decisions.
			public int Metric2;
			public int Metric3;
			public int Metric4;
			public int Metric5;
		}

		// Layout of the routing table.
		[StructLayout(LayoutKind.Sequential)]
		private struct MIB_IPFORWARDTABLE
		{
			public int dwNumEntries;
			public MIB_IPFORWARDROW route;
		}

		/// <summary>
		/// Data class to hold relevant network interface attributes.
		/// </summary>
		private class InterfaceInfo
		{
			public IPAddress IpAddress { get; set; }
			public string Description { get; set; }
			public int Metric { get; set; }

			public override string ToString() => $"{IpAddress} ({Description})";
		}

		/// <summary>
		/// This class retrieves the routing table to generate a lookup table for route metrics,
		/// which can be queried.
		/// </summary>
		private sealed class RoutingTableProxy
		{
			// We use an unmanaged function from this DLL that is part of Windows.
			[DllImport("iphlpapi.dll")]
			private static extern int GetIpForwardTable(IntPtr pIpForwardTable, ref int pdwSize, bool bOrder);
			
			// Lookup table (keyed by interface index) containing the *lowest* interface route cost
			// for each network interface. Data is from the routing table.
			private readonly Dictionary<uint, int> _interfaceCost;

			public RoutingTableProxy()
			{
				_interfaceCost = new Dictionary<uint, int>();
				LoadRoutingTable();
			}

			/// <summary>
			/// Get the metric (route cost) for the given interface.
			/// </summary>
			/// <remarks>
			/// When the lookup table was populated, care was taken to ensure that it would store
			/// only the lowest value for each interface.
			/// </remarks>
			public int GetMetricForInterface(uint interfaceIndex)
			{
				_interfaceCost.TryGetValue(interfaceIndex, out var metric);
				return metric;
			}

			// This method owns the buffer holding a copy of the routing table.
			// The entire buffer lifecycle is here: create it, fill it with routing table
			// data, parse it into a lookup table, and finally free it.
			//
			private void LoadRoutingTable()
			{
				const int ERROR_INSUFFICIENT_BUFFER = 122; // from winerror.h
				
				var routingTableBuf = IntPtr.Zero;

				try
				{
					int size = 0;

					// First call to get the size. We expect an error, and 'size'
					// will then contain the value of needed buffer length.
					int result = GetIpForwardTable(IntPtr.Zero, ref size, true);
					if (result != ERROR_INSUFFICIENT_BUFFER)
					{
						Debug.WriteLine($"AndroidSynchronization, error ({result}) getting size");
						throw new Win32Exception(result);
					}

					routingTableBuf = Marshal.AllocHGlobal(size);

					// Second call gets the table.
					result = GetIpForwardTable(routingTableBuf, ref size, true);
					if (result != 0)
					{
						Debug.WriteLine($"AndroidSynchronization, error ({result}) getting buffer or table");
						throw new Win32Exception(result);
					}

					// Parse the buffer: for each row (which is a route) get the interface
					// metric (route cost). We store the lowest cost metric for any given interface.

					var table = Marshal.PtrToStructure<MIB_IPFORWARDTABLE>(routingTableBuf);
					var rowPtr = IntPtr.Add(routingTableBuf, Marshal.OffsetOf<MIB_IPFORWARDTABLE>("route").ToInt32());

					for (int i = 0; i < table.dwNumEntries; i++)
					{
						var row = Marshal.PtrToStructure<MIB_IPFORWARDROW>(rowPtr);

						if (!_interfaceCost.TryGetValue(row.InterfaceIndex, out var metric) || row.RouteCost < metric)
							_interfaceCost[row.InterfaceIndex] = row.RouteCost;
						rowPtr = IntPtr.Add(rowPtr, Marshal.SizeOf<MIB_IPFORWARDROW>());
					}
				}
				finally
				{
					if (routingTableBuf != IntPtr.Zero)
					{
						Marshal.FreeHGlobal(routingTableBuf);
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
			RoutingTableProxy proxy;

			try
			{
				proxy = new RoutingTableProxy();
			}
			catch (Exception e)
			{
				ErrorReport.ReportNonFatalExceptionWithMessage(e,
					LocalizationManager.GetString("AndroidSynchronization.RoutingTableError",
						"Error getting routing table for Android synchronization."));
				proxy = null;
			}

			// Get our local IP address, which we will advertise.
			var localIp = GetInterfaceStackWillUse(proxy, s =>
				MessageBox.Show(parent, s, Program.kProduct));

			if (localIp == null)
				return;

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
			};
			dlg.ShowDialog(parent);
		}

		// Survey the network interfaces, determine which one (if any) the network stack
		// will use for network traffic, and return the appropriate IP address.
		//   - During the assessment the current leading Wi-Fi candidate will be held in
		//     'wifiInterface' and similarly the current best candidate for Ethernet will be
		//     in 'ethernetInterface'.
		//   - After assessment return the winner's IPv4 address, or null if there
		//     is no winner.
		//
		private static IPAddress GetInterfaceStackWillUse(RoutingTableProxy proxy,
			Action<string> reportProblem)
		{
			InterfaceInfo wifiInterface = null;
			InterfaceInfo ethernetInterface = null;
			
			// Retrieve all network interfaces that are *active*.
			var allOperationalNetworks = NetworkInterface.GetAllNetworkInterfaces()
				.Where(ni => ni.OperationalStatus == OperationalStatus.Up).ToArray();

			if (!allOperationalNetworks.Any())
			{
				reportProblem(LocalizationManager.GetString("AndroidSynchronization.NetworkingRequired",
					"Android synchronization requires your computer to have networking enabled."));
				return null;
			}

			// Get key attributes of active network interfaces.
			foreach (var ni in allOperationalNetworks)
			{
				// If we can't get IP or IPv4 properties for this interface, skip it.
				var ipProps = ni.GetIPProperties();
				var ipv4Props = ipProps?.GetIPv4Properties();
				if (ipv4Props == null)
					continue;

				var interfaceIndex = (uint)ipv4Props.Index;

				// We don't consider IPv6 so filter for IPv4 ('InterNetwork')...
				foreach (var ip in ipProps.UnicastAddresses
					.Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork))
				{
					// Of these, we care only about Wi-Fi and Ethernet.
					int currentInterfaceMetric;
					if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
					{
						// If we failed to retrieve the routing table info, we have no basis
						// for evaluating which interface is best, but generally we assume that
						// wireless is better than wired, so we'll treat this (the first one we
						// encounter) as our winner.
						currentInterfaceMetric = proxy?.GetMetricForInterface(interfaceIndex) ?? 0;

						// Save interface if it's the first or its metric is lowest we've seen.
						if (wifiInterface == null || currentInterfaceMetric < wifiInterface.Metric)
						{
							wifiInterface = new InterfaceInfo
							{
								IpAddress = ip.Address,
								Description = ni.Description,
								Metric = currentInterfaceMetric
							};
						}
					}
					else if (wifiInterface == null && ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
					{
						currentInterfaceMetric = proxy?.GetMetricForInterface(interfaceIndex) ?? 0;

						// Save interface if it's the first or its metric is lowest we've seen.
						if (ethernetInterface == null || currentInterfaceMetric < ethernetInterface.Metric)
						{
							ethernetInterface = new InterfaceInfo
							{
								IpAddress = ip.Address,
								Description = ni.Description,
								Metric = currentInterfaceMetric
							};
						}
					}
				}
			}

			// Active network interfaces have all been assessed.
			// Now choose the winner, if there is one:
			// Lowest cost active Wi-Fi interface, if any, always wins;
			// otherwise, the best active Ethernet interface.

			if (wifiInterface != null)
			{
				Logger.WriteEvent($"Found a Wi-Fi network for Android synchronization: {wifiInterface}");
				return wifiInterface.IpAddress;
			}
			if (ethernetInterface != null)
			{
				Logger.WriteEvent($"Found a wired network for Android synchronization: {ethernetInterface}");
				return ethernetInterface.IpAddress;
			}

			reportProblem(LocalizationManager.GetString(
				"AndroidSynchronization.NoInterNetworkIPAddress",
				"Sorry, your network adapter does not have a valid IP address for " +
				"connecting to other networks. If you are not sure how to fix this, " +
				"please seek technical help."));
			return null;
		}
	}
}
