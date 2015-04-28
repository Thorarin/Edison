using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Edison.Plugin.Common.Lighting;
using Edison.Plugin.Common.Net;
using MoreLinq;

namespace Edison.Plugin.MiLight
{
    public class MiLightPlugin
    {
        private readonly INetworkService _networkService;

        public MiLightPlugin(INetworkService networkService)
        {
            _networkService = networkService;
        }

        public async Task<IEnumerable<WifiBridgeInfo>> DiscoverAsync(TimeSpan timeout)
        {
            var client = _networkService.CreateUdpBroadcastClient(48899);
            var receivedPackets = new List<UdpReceiveResult>();

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            DiscoverReceive(client, receivedPackets, tokenSource.Token);

            // ReSharper disable MethodSupportsCancellation
            int loops = (int)timeout.TotalMilliseconds / 50;
            byte[] discoveryPacket = Encoding.UTF8.GetBytes("Link_Wi-Fi");
            IPEndpoint endpoint = new IPEndpoint(IPAddress.Broadcast, 48899);
            for (int i = 0; i < loops; i++)
            {
                await client.SendAsync(discoveryPacket, endpoint);
                await Task.Delay(50);
            }

            await Task.Delay(200);
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

        public async Task<ILightingController> GetControllerAsync(WifiBridgeInfo bridgeInfo)
        {
            var controller = new Controller(_networkService);
            controller.RemoteEndpoint = new IPEndpoint(bridgeInfo.Address, 8899);

            await controller.InitializeAsync();

            return controller;
        }
    }
}
