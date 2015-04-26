using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Edison.Plugin.Common.Net;

namespace Edison.ConsoleApp.Net
{
    internal class UdpClient : IUdpClient
    {
        private readonly System.Net.Sockets.UdpClient _client;

        public UdpClient(int localPort)
        {
            //_client = new System.Net.Sockets.UdpClient(new IPEndPoint(IPAddress.Any, 48899))
            _client = new System.Net.Sockets.UdpClient(localPort);
            {
                //EnableBroadcast = true,
                //DontFragment = true,
                
            };
        }

        public UdpClient(IPEndpoint localEndpoint)
        {
            _client = new System.Net.Sockets.UdpClient(new IPEndPoint(System.Net.IPAddress.Parse(localEndpoint.Address), localEndpoint.Port));
            //_client = new System.Net.Sockets.UdpClient(localEndpoint.Host, localEndpoint.Port);
        }

        public Task ConnectAsync(string remoteHost, int remotePort)
        {
            _client.Connect(remoteHost, remotePort);
            return Task.FromResult(true);
        }

        public async Task SendAsync(byte[] datagram)
        {
            await _client.SendAsync(datagram, datagram.Length);
        }

        public async Task SendAsync(byte[] datagram, IPEndpoint remoteEndpoint)
        {
            await _client.SendAsync(datagram, datagram.Length, new IPEndPoint(System.Net.IPAddress.Parse(remoteEndpoint.Address), remoteEndpoint.Port));
        }

        public async Task<Edison.Plugin.Common.Net.UdpReceiveResult> ReceiveAsync()
        {
            var received = await _client.ReceiveAsync();
            return new Edison.Plugin.Common.Net.UdpReceiveResult(
                received.Buffer,
                new IPEndpoint(received.RemoteEndPoint.Address.ToString(), received.RemoteEndPoint.Port));
        }


        public bool EnableBroadcast
        {
            get { return _client.EnableBroadcast; }
            set { _client.EnableBroadcast = value; }
        }

        public bool DontFragment
        {
            get { return _client.DontFragment; }
            set { _client.DontFragment = value; }
        }

        public void Dispose()
        {
            _client.Close();
        }
    }
}
