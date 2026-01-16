using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace HearThis.Communication
{
	/// <summary>
	/// This class retrieves the routing table to generate a lookup table for route metrics,
	/// which can be queried.
	/// </summary>
	public class RoutingTableProxy : INetworkInterfaceRoutingEvaluator
	{
		/// <summary>
		/// Layout of a row in the IPv4 routing table. This struct maps to MIB_IPFORWARDROW in
		/// the Windows API; fields have been renamed here for clarity. (Most are not used.)
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct MIB_IPFORWARDROW
		{
			public uint Destination;
			public uint SubnetMask;
			public uint Policy;
			public uint NextHop;
			/// <summary>
			/// The index used for lookup (correlates to the IPv4 Props index).
			/// Corresponds to dwForwardIfIndex in the original Windows struct.
			/// </summary>
			public uint InterfaceIndex;
			public int Type;
			public int Protocol;
			public int Age;
			public uint NextHopAS;
			/// <summary>
			/// This is the metric we care about for evaluating which interface is best.
			/// Corresponds to dwForwardMetric1 in the original Windows struct.
			/// </summary>
			public int RouteCost;
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
}
