using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace HearThis.Communication
{
	/// <summary>
	/// Interface to abstract the properties of a network interface used by HearThis.
	/// </summary>
	public interface INetworkInterface
	{
		string Name { get; }
		string Description { get; }
		string Id { get; }
		OperationalStatus Status { get; }
		NetworkInterfaceType Type { get; }
		bool IsIPV4 { get; }
		IEnumerable<IPAddress> IPV4Addresses { get;}
		uint IPv4InterfaceIndex { get; }
	}
}
