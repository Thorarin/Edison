using System;
using System.Threading.Tasks;

namespace Edison.Plugin.Common.Net
{
    public interface IUdpBroadcastClient : IDisposable
    {
        Task SendAsync(byte[] datagram, IPEndpoint remoteEndpoint);
        Task<UdpReceiveResult> ReceiveAsync();
    }
}
