namespace HearThis.Communication
{
	public interface INetworkInterfaceRoutingEvaluator
	{
		int GetMetricForInterface(uint interfaceIndex);
	}
}
