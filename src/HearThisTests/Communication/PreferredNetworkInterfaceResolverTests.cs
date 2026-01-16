using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using HearThis.Communication;
using NUnit.Framework;
using static System.Net.NetworkInformation.NetworkInterfaceType;
using static System.Net.NetworkInformation.OperationalStatus;
using static HearThis.Communication.PreferredNetworkInterfaceResolver;

namespace HearThisTests.Communication
{
	[TestFixture]
	public class PreferredNetworkInterfaceResolverTests
	{
		private class TestNetworkInterface : INetworkInterface
		{
			public static uint _indexCounter = 0;
			
			private readonly IEnumerable<IPAddress> _ipv4Addresses;

			public TestNetworkInterface(bool isIpv4, IList<IPAddress> addresses)
			{
				IsIPV4 = isIpv4;
				if (isIpv4)
				{
					Assert.That(addresses, Is.Not.Empty);
					IPv4InterfaceIndex = _indexCounter++;
					_ipv4Addresses = addresses;
				}
			}
			
			public string Name => $"{Id} ({Type})";
			public string Description => Name;
			public string Id { get; } = Guid.NewGuid().ToString();
			public OperationalStatus Status { get; internal set; }
			public NetworkInterfaceType Type { get; internal set; }
			public bool IsIPV4 { get; }

			public IEnumerable<IPAddress> IPV4Addresses => IsIPV4 ? _ipv4Addresses :
				throw new InvalidOperationException("Do not access " +
				$"{nameof(IPV4Addresses)} on a non-IPV4 network interface.");

			public uint IPv4InterfaceIndex { get; }
		}

		private class TestNetworkInterfaceFactory : INetworkInterfaceFactory
		{
			private readonly IList<INetworkInterface> _interfaces;
			public TestNetworkInterfaceFactory(IList<INetworkInterface> interfaces)
			{
				_interfaces = interfaces;
			}

			public IEnumerable<INetworkInterface> GetAll() => _interfaces;
		}

		private class TestRouteEvaluator : INetworkInterfaceRoutingEvaluator
		{
			private readonly IDictionary<uint, int> _table;

			public TestRouteEvaluator(IDictionary<uint, int> table)
			{
				_table = table;
			}

			public int GetMetricForInterface(uint interfaceIndex)
			{
				return _table.TryGetValue(interfaceIndex, out var value) ?
					value : int.MaxValue;
			}
		}

		[SetUp]
		public void SetUp()
		{
			TestNetworkInterface._indexCounter = 0;
		}

		[Test]
		public void GetBestActiveInterface_NoInterfaces_FailsWithNetworkingNotEnabled()
		{
			var resolver = new PreferredNetworkInterfaceResolver(
				null, new TestNetworkInterfaceFactory(Array.Empty<INetworkInterface>()));

			var result = resolver.GetBestActiveInterface(out var reason);

			Assert.That(result, Is.Null);
			Assert.That(reason, Is.EqualTo(FailureReason.NetworkingNotEnabled));
		}
		
		[TestCase(Ethernet)]
		[TestCase(Wireless80211)]
		[TestCase(Ethernet, Wireless80211, FastEthernetT)]
		public void GetBestActiveInterface_NoActiveInterfaces_FailsWithNetworkingNotEnabled(
			params NetworkInterfaceType[] types)
		{
			var interfaces = new List<INetworkInterface>();
			for (int i = 0; i < types.Length; i++)
			{
				var type = types[i];
				var isIpV4 = type == Ethernet || type == Wireless80211;
				var addresses = new List<IPAddress>
				{
					new IPAddress(new byte[] { 192, 162, 0, (byte)i })
				};
				
				interfaces.Add(new TestNetworkInterface(isIpV4, addresses)
				{
					Status = Down,
					Type = type,
				});
			}

			var resolver = new PreferredNetworkInterfaceResolver(null,
				new TestNetworkInterfaceFactory(interfaces));

			var result = resolver.GetBestActiveInterface(out var reason);

			Assert.That(result, Is.Null);
			Assert.That(reason, Is.EqualTo(FailureReason.NetworkingNotEnabled));
		}

		[Test]
		public void GetBestActiveInterface_NoIPv4_ReturnsNullWithNoIPv4Address()
		{
			var interfaces = new List<INetworkInterface>
			{
				new TestNetworkInterface(false, Array.Empty<IPAddress>())
					{ Status = Up, Type = Ethernet }
			};

			var resolver = new PreferredNetworkInterfaceResolver(null,
				new TestNetworkInterfaceFactory(interfaces));
			var result = resolver.GetBestActiveInterface(out var reason);

			Assert.That(result, Is.Null);
			Assert.That(reason, Is.EqualTo(FailureReason.NoInterNetworkIPAddress));
		}

		[TestCase(true)]
		[TestCase(false)]
		public void GetBestActiveInterface_BothWifiAndEthernetInterfacesActiveButNoEvaluator_PicksFirstWifi(
			bool includeSecondWifi)
		{
			var interfaces = new List<INetworkInterface>
			{
				new TestNetworkInterface(
					true,
					new[] { new IPAddress(new byte[] {192, 168, 0, 5 }) })
				{
					Status = Up,
					Type = Ethernet
				},
				new TestNetworkInterface(
					true,
					new[] { new IPAddress(new byte[] {192, 168, 0, 10 }) })
				{
					Status = Up,
					Type = Wireless80211
				}
			};

			if (includeSecondWifi)
			{
				interfaces.Add(new TestNetworkInterface(
					true,
					new[] { new IPAddress(new byte[] { 192, 168, 0, 15 }) })
				{
					Status = Up,
					Type = Wireless80211
				});
			}

			var resolver = new PreferredNetworkInterfaceResolver(null,
				new TestNetworkInterfaceFactory(interfaces));

			var result = resolver.GetBestActiveInterface(out var reason);

			Assert.That(result, Is.EqualTo(IPAddress.Parse("192.168.0.10")));
			Assert.That(reason, Is.EqualTo(FailureReason.None));
		}

		[TestCase(true)]
		[TestCase(false)]
		public void GetBestActiveInterface_OnlyEthernetInterfacesActiveButNoEvaluator_PicksFirstEthernet(
			bool includeSecondEthernet)
		{
			var interfaces = new List<INetworkInterface>
			{
				new TestNetworkInterface(
					true,
					new[] { new IPAddress(new byte[] {192, 168, 0, 5 }) })
				{
					Status = Up,
					Type = Ethernet
				},
			};

			if (includeSecondEthernet)
			{
				interfaces.Add(new TestNetworkInterface(
					true,
					new[] { new IPAddress(new byte[] { 192, 168, 0, 7 }) })
				{
					Status = Up,
					Type = Ethernet
				});
			}

			var resolver = new PreferredNetworkInterfaceResolver(null,
				new TestNetworkInterfaceFactory(interfaces));

			var result = resolver.GetBestActiveInterface(out var reason);

			Assert.That(result, Is.EqualTo(IPAddress.Parse("192.168.0.5")));
			Assert.That(reason, Is.EqualTo(FailureReason.None));
		}

		[Test]
		public void GetBestActiveInterface_EvaluatorSaysSecondEthernetInterfaceIsBest_PicksSecondEthernet()
		{
			var interfaces = new List<INetworkInterface>
			{
				new TestNetworkInterface(
					true,
					new[] { new IPAddress(new byte[] { 192, 165, 0, 5 }) })
				{
					Status = Up,
					Type = Ethernet
				},
				new TestNetworkInterface(
					true,
					new[] { new IPAddress(new byte[] { 192, 168, 1, 7 }) })
				{
					Status = Up,
					Type = Ethernet
				}
			};

			var table = new Dictionary<uint, int>
			{
				{ 0, 1000 }, // First one has a higher metric
				{ 1, 2 }
			};

			var resolver = new PreferredNetworkInterfaceResolver(new TestRouteEvaluator(table),
				new TestNetworkInterfaceFactory(interfaces));

			var result = resolver.GetBestActiveInterface(out var reason);

			Assert.That(result, Is.EqualTo(IPAddress.Parse("192.168.1.7")));
			Assert.That(reason, Is.EqualTo(FailureReason.None));
		}

		[TestCase(1)]
		[TestCase(2)]
		public void GetBestActiveInterface_EvaluatorIndicatesEthernetInterfaceIsBest_PicksWifi(
			int cWiFi)
		{
			var interfaces = new List<INetworkInterface>
			{
				new TestNetworkInterface(
					true,
					new[] { new IPAddress(new byte[] {192, 168, 0, 5 }) })
				{
					Status = Up,
					Type = Ethernet
				},
			};

			for (var i = 1; i <= cWiFi; i++)
			{
				interfaces.Add(new TestNetworkInterface(
					true,
					new[] { new IPAddress(new byte[] { 192, 168, 0, (byte)(10 + i) }) })
				{
					Status = Up,
					Type = Wireless80211
				});
			}

			var table = new Dictionary<uint, int>
			{
				{ 0, 1 }, // Ethernet has a lower metric
				{ 1, 100 }, // Wi-Fi has a higher metric
				{ 2, 50 } // Wi-Fi 2 (if present) has a lower metric than Wi-Fi 1, but still worse than Ethernet.
			};

			var resolver = new PreferredNetworkInterfaceResolver(new TestRouteEvaluator(table),
				new TestNetworkInterfaceFactory(interfaces));

			var result = resolver.GetBestActiveInterface(out var reason);

			Assert.That(result, Is.EqualTo(IPAddress.Parse($"192.168.0.{10 + cWiFi}")));
			Assert.That(reason, Is.EqualTo(FailureReason.None));
		}
	}
}
