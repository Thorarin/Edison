using System;
using System.Threading.Tasks;
using Edison.Plugin.Common.Lighting;
using Edison.Plugin.Common.Net;

namespace Edison.Plugin.MiLight
{
    public class Controller : ILightingController
    {
        private readonly INetworkService _networkService;
        private IUdpClient _udpClient;

        public Controller(INetworkService networkService)
        {
            _networkService = networkService;
        }

        public IPEndpoint RemoteEndpoint { get; set; }

        public void Initialize()
        {
            _udpClient = _networkService.CreateUdpClient(RemoteEndpoint);
        }

        public Task TurnOnAsync(ILightingZone zone)
        {
            throw new NotImplementedException();
        }

        public Task TurnOffAsync(ILightingZone zone)
        {
            throw new NotImplementedException();
        }

        public Task SetBrightnessAsync(ILightingZone zone, Brightness brightness)
        {
            throw new NotImplementedException();
        }

        public Task SetColorAsync(ILightingZone zone, Color color)
        {
            throw new NotImplementedException();
        }

        private Task SendAsync(byte[] data)
        {
            return _udpClient.SendAsync(data);
        }
    }
}
