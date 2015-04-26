using Edison.Plugin.Common.Net;

namespace Edison.ConsoleApp.Net
{
    public class NetworkService : INetworkService
    {
        public IUdpClient CreateUdpClient(int localPort)
        {
            return new UdpClient(localPort);
        }

        public IUdpClient CreateUdpClient(IPEndpoint localEndpoint)
        {
            return new UdpClient(localEndpoint);
        }

        public IUdpBroadcastClient CreateUdpBroadcastClient(int localPort)
        {
            return new UdpBroadcastClient(localPort);
        }
    }
}
