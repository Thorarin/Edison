using System;
using System.Threading.Tasks;

namespace Edison.Plugin.Common.Net
{
    public interface IUdpClient : IDisposable
    {
        Task ConnectAsync(string remoteHost, int remotePort);
        Task SendAsync(byte[] datagram);
        Task SendAsync(byte[] datagram, IPEndpoint remoteEndpoint);
        Task<UdpReceiveResult> ReceiveAsync();

        bool EnableBroadcast { get; set; }
        bool DontFragment { get; set; }
    }
}
