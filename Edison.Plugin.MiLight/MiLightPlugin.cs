using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Edison.Plugin.Common.Net;
using MoreLinq;

namespace Edison.Plugin.MiLight
{
    public class MiLightPlugin
    {
        private readonly INetworkService _networkService;
        private readonly IUdpClient _udpClient;

        public MiLightPlugin(INetworkService networkService)
        {
            _networkService = networkService;

            //_udpClient = udpClient;
            //udpClient.ConnectAsync("192.168.0.255", 48899);
        }

        public async Task<IEnumerable<WifiBridgeInfo>> DiscoverAsync(TimeSpan timeout)
        {
            var client = _networkService.CreateUdpBroadcastClient(48899);
            var receivedPackets = new List<UdpReceiveResult>();

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            DiscoverReceive(client, receivedPackets, tokenSource.Token);

            // ReSharper disable MethodSupportsCancellation
            int loops = (int)timeout.TotalMilliseconds / 50;
            IPEndpoint endpoint = new IPEndpoint(IPAddress.Broadcast, 48899);
            for (int i = 0; i < loops; i++)
            {
                await client.SendAsync(new byte[] { 0x4C, 0x69, 0x6E, 0x6B, 0x5F, 0x57, 0x69, 0x2D, 0x46, 0x69 }, endpoint);
                await Task.Delay(50);
            }

            // ReSharper restore MethodSupportsCancellation
            tokenSource.Cancel();
            client.Dispose();

            return ParseReceivedDiscoveryPackets(receivedPackets);
        }

        private async void DiscoverReceive(IUdpBroadcastClient client, ICollection<UdpReceiveResult> results, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = await client.ReceiveAsync();
                    results.Add(result);
                }
                catch
                {
                }
            }
        }

        private IEnumerable<WifiBridgeInfo> ParseReceivedDiscoveryPackets(IList<UdpReceiveResult> receivedPackets)
        {
            receivedPackets = receivedPackets.DistinctBy(result => result.RemoteEndpoint.Address).ToList();

            var results = new List<WifiBridgeInfo>(receivedPackets.Count);
            var regex = new Regex(@"(?<IP>\d{1,3}\.\d{1,3}\.\d{1,3}.\d{1,3}),(?<PhysicalAddress>[0-9A-F]+),");
            foreach (var packet in receivedPackets)
            {
                var match = regex.Match(Encoding.UTF8.GetString(packet.Data, 0, packet.Data.Length));
                if (match.Success)
                {
                    results.Add(new WifiBridgeInfo(match.Groups["IP"].Value, match.Groups["PhysicalAddress"].Value));
                }
            }

            return results;
        }

        public async Task TurnOffAsync()
        {
            await _udpClient.SendAsync(new byte[] { 0x41, 0x00 });
        }

        public async Task TurnOnAsync()
        {
            await _udpClient.SendAsync(new byte[] { 0x42, 0x00 });
        }

        public async Task SetBrightness(int brightness)
        {
            await _udpClient.SendAsync(new byte[] { 0x4e, (byte)brightness });
        }

        public async Task SetColor(int color)
        {
            await _udpClient.SendAsync(new byte[] { 0x40, (byte)color });
        }


    }
}
