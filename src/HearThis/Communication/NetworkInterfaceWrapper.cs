using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace HearThis.Communication
{
	/// <summary>
	/// This implements a wrapper around the parts of the .NET NetworkInterface class needed by
	/// HearThis. Together with <see cref="NetworkInterfaceFactory"/>, this facilitates unit
	/// testing, since the classes involved are essentially sealed (abstract classes with
	/// protected constructors and only internal implementations).
	/// </summary>
	public class NetworkInterfaceWrapper : INetworkInterface
	{
		private readonly NetworkInterface _inner;
		private readonly IPInterfaceProperties _ipProperties;
		private readonly IPv4InterfaceProperties _ipv4Properties;

		public NetworkInterfaceWrapper(NetworkInterface inner)
		{
			_inner = inner ?? throw new ArgumentNullException(nameof(inner));
			_ipProperties = _inner.GetIPProperties();
			_ipv4Properties = _ipProperties?.GetIPv4Properties();
		}

		public string Name => _inner.Name;
		public string Description => _inner.Description;
		public string Id => _inner.Id;
		public OperationalStatus Status => _inner.OperationalStatus;
		public NetworkInterfaceType Type => _inner.NetworkInterfaceType;
		public bool IsIPV4 => _ipv4Properties != null;

		public IEnumerable<IPAddress> IPV4Addresses
		{
			get
			{
				if (_ipProperties == null)
				{
					throw new InvalidOperationException("Do not access " +
						$"{nameof(IPV4Addresses)} on a non-IPV4 network interface.");
				}
				return _ipProperties.UnicastAddresses
					.Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork)
					.Select(a => a.Address);
			}
		}
		
		public uint IPv4InterfaceIndex
		{
			get
			{
				if (_ipv4Properties == null)
				{
					throw new InvalidOperationException("Do not access " +
						$"{nameof(IPv4InterfaceIndex)} on a non-IPV4 network interface.");
				}

				return (uint)_ipv4Properties.Index;
			}
		}
	}
}
