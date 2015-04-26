using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Edison.Plugin.Common.Net;
using UdpReceiveResult = Edison.Plugin.Common.Net.UdpReceiveResult;

namespace Edison.ConsoleApp.Net
{
    public class UdpBroadcastClient : IUdpBroadcastClient
    {
        private readonly UdpClient[] _clients;
        private readonly Task<UdpReceiveResult>[] _receiveTasks;
        private readonly IPAddress[] _localAddresses;

        public UdpBroadcastClient(int localPort)
        {
            _localAddresses = NetworkInterface.GetAllNetworkInterfaces().
                Where(iface => iface.NetworkInterfaceType != NetworkInterfaceType.Loopback).
                SelectMany(iface => iface.GetIPProperties().UnicastAddresses).
                Where(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork).
                Select(addr => IPAddress.Parse(addr.Address.ToString())).
                ToArray();

            _clients = _localAddresses.Select(addr =>
                new UdpClient(new IPEndpoint(addr, localPort))
                {
                    EnableBroadcast = true,
                    DontFragment = true
                }).ToArray();
            _receiveTasks = new Task<UdpReceiveResult>[_clients.Length];
        }

        public async Task SendAsync(byte[] datagram, IPEndpoint remoteEndpoint)
        {
            foreach (UdpClient client in _clients)
            {
                await client.SendAsync(datagram, remoteEndpoint);
            }
        }

        public async Task<UdpReceiveResult> ReceiveAsync()
        {
            Task<UdpReceiveResult> receivedTask;

            do
            {
                for (int i = 0; i < _clients.Length; i++)
                {
                    if (_receiveTasks[i] == null)
                        _receiveTasks[i] = _clients[i].ReceiveAsync();
                }

                receivedTask = await Task.WhenAny(_receiveTasks);

                for (int i = 0; i < _receiveTasks.Length; i++)
                {
                    if (_receiveTasks[i] == receivedTask)
                        _receiveTasks[i] = null;
                }
            } while (_localAddresses.Contains(receivedTask.Result.RemoteEndpoint.Address));
            
            return receivedTask.Result;
        }

        public void Dispose()
        {
            foreach (var client in _clients)
            {
                client.Dispose();
            }
        }
    }
}
