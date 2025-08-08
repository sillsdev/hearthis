using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using L10NSharp;
using SIL.Reporting;
using static System.Net.NetworkInformation.NetworkInterfaceType;

namespace HearThis.Communication
{
	public class PreferredNetworkInterfaceResolver
	{
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
		
		private readonly INetworkInterfaceRoutingEvaluator _routeEvaluator;
		private readonly INetworkInterfaceFactory _networkInterfaceFactory;

		public enum FailureReason
		{
			None,
			NetworkingNotEnabled,
			NoInterNetworkIPAddress,
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PreferredNetworkInterfaceResolver"/>
		/// intended for production use. It uses the "real" RoutingTableProxy and
		/// the real call to <see cref="NetworkInterface.GetAllNetworkInterfaces"/>
		/// </summary>
		public PreferredNetworkInterfaceResolver()
		{
			// To set up for getting the local IP address, get routing table data.
			try
			{
				_routeEvaluator = new RoutingTableProxy();
			}
			catch (Exception e)
			{
				ErrorReport.ReportNonFatalExceptionWithMessage(e,
					LocalizationManager.GetString("AndroidSynchronization.RoutingTableError",
						"Error getting routing table for Android synchronization."));
			}

			_networkInterfaceFactory = new NetworkInterfaceFactory();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PreferredNetworkInterfaceResolver"/>
		/// intended for unit testing.
		/// </summary>
		/// <param name="routeEvaluator">A mocked <see cref="INetworkInterfaceRoutingEvaluator"/>
		/// </param>
		/// <param name="networkInterfaceFactory">A mocked object to get mocked network interfaces
		/// </param>
		public PreferredNetworkInterfaceResolver(INetworkInterfaceRoutingEvaluator routeEvaluator,
			INetworkInterfaceFactory networkInterfaceFactory)
		{
			_routeEvaluator = routeEvaluator;
			_networkInterfaceFactory = networkInterfaceFactory;
		}

		/// <summary>
		/// Survey the network interfaces, determine which one (if any) is the best one to use for
		/// network traffic, and return its .
		/// </summary>
		/// <param name="failureReason">If no usable network interface is found, this will be set
		/// to a value indicating the reason.</param>
		/// <returns>The IPv4 address of the best network interface or <c>null</c> if no usable
		/// network interface is found</returns>
		public IPAddress GetBestActiveInterface(out FailureReason failureReason)
		{
			// During the assessment the current leading Wi-Fi candidate will be held in
			// `wifiInterface` and similarly the current best candidate for Ethernet will be
			// in `ethernetInterface`.
			InterfaceInfo wifiInterface = null;
			InterfaceInfo ethernetInterface = null;

			// Retrieve all network interfaces that are *active*.
			var allOperationalNetworks = _networkInterfaceFactory.GetAll()
				.Where(ni => ni.Status == OperationalStatus.Up).ToArray();

			if (!allOperationalNetworks.Any())
			{
				failureReason = FailureReason.NetworkingNotEnabled;
				return null;
			}

			// Get key attributes of active network interfaces.
			foreach (var ni in allOperationalNetworks)
			{
				// If we can't get IP or IPv4 properties for this interface, skip it.
				//var ipProps = ni.GetIPProperties();
				if (!ni.IsIPV4)
					continue;

				var interfaceIndex = ni.IPv4InterfaceIndex;

				// We don't consider IPv6 so filter for IPv4 ('InterNetwork')...
				foreach (var ip in ni.IPV4Addresses)
				{
					// Of these, we care only about Wi-Fi and Ethernet.
					if (ni.Type == Wireless80211)
					{
						// We always assume that wireless is better than wired, so once we get a
						// Wi-Fi candidate, we ignore any subsequent Ethernet candidates.
						RememberBestInterface(ref wifiInterface, ni, ip, interfaceIndex);
					}
					else if (wifiInterface == null && ni.Type == Ethernet)
					{
						RememberBestInterface(ref ethernetInterface, ni, ip, interfaceIndex);
					}
				}
			}

			// Active network interfaces have all been assessed.
			// Now choose the winner, if there is one:
			// Lowest cost active Wi-Fi interface, if any, always wins;
			// otherwise, the best active Ethernet interface.
			failureReason = FailureReason.None;
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

			failureReason = FailureReason.NoInterNetworkIPAddress;
			return null;
		}

		/// <summary>
		/// Save the interface if it's the first or its metric is lowest we've seen.
		/// </summary>
		/// <param name="iInfo">The existing interface (null) of a particular type (Wi-Fi or
		/// Ethernet)</param>
		/// <param name="ni">The network interface being evaluated</param>
		/// <param name="ipAddress">The IP address of the interface</param>
		/// <param name="interfaceIndex"></param>
		private void RememberBestInterface(ref InterfaceInfo iInfo, INetworkInterface ni,
			IPAddress ipAddress, uint interfaceIndex)
		{
			// If we failed to retrieve the routing table info, we have no basis
			// for evaluating which interface is best, so we'll treat this (the first one we
			// encounter) as our winner.
			var metric = _routeEvaluator?.GetMetricForInterface(interfaceIndex) ?? 0;

			if (iInfo == null || metric < iInfo.Metric)
				iInfo = new InterfaceInfo
				{
					IpAddress = ipAddress,
					Description = ni.Description,
					Metric = metric
				};
		}
	}
}
