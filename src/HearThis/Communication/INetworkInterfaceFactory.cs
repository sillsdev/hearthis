using System.Collections.Generic;

namespace HearThis.Communication
{
	/// <summary>
	/// Interface to allow abstraction of logic to retrieve network interfaces.
	/// </summary>
	public interface INetworkInterfaceFactory
	{
		IEnumerable<INetworkInterface> GetAll();
	}
}
