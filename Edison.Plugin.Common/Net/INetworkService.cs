namespace Edison.Plugin.Common.Net
{
    public interface INetworkService
    {
        IUdpClient CreateUdpClient(int localPort);
        IUdpClient CreateUdpClient(IPEndpoint localEndpoint);
        IUdpBroadcastClient CreateUdpBroadcastClient(int localPort);
    }
}
