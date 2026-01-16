using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace HearThis.Communication
{
	/// <summary>
	/// This implements a wrapper around <see cref="NetworkInterface.GetAllNetworkInterfaces()"/>.
	/// Together with <see cref="NetworkInterfaceWrapper"/>, this facilitates unit
	/// testing, since the classes involved are essentially sealed (abstract classes with
	/// protected constructors and only internal implementations).
	/// </summary>
	public class NetworkInterfaceFactory : INetworkInterfaceFactory
	{
		public IEnumerable<INetworkInterface> GetAll()
		{
			return NetworkInterface.GetAllNetworkInterfaces()
				.Select(nic => new NetworkInterfaceWrapper(nic));
		}
	}
}
