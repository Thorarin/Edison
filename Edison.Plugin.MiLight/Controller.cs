using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Edison.Plugin.Common.Lighting;
using Edison.Plugin.Common.Net;

namespace Edison.Plugin.MiLight
{
    public class Controller : ILightingController
    {
        private readonly INetworkService _networkService;
        private IUdpClient _udpClient;
        private static readonly List<ILightingZone> _zones;
        private static readonly IDictionary<ILightingZone, ZoneCommandInfo> _zoneCommands;

        static Controller()
        {
            _zones = new List<ILightingZone>
            {
                new MilightZone(MilightZoneType.White, 0),
                new MilightZone(MilightZoneType.White, 1),
                new MilightZone(MilightZoneType.White, 2),
                new MilightZone(MilightZoneType.White, 3),
                new MilightZone(MilightZoneType.White, 4),
                new MilightZone(MilightZoneType.Rgb, 1),
                new MilightZone(MilightZoneType.Rgbw, 0),
                new MilightZone(MilightZoneType.Rgbw, 1),
                new MilightZone(MilightZoneType.Rgbw, 2),
                new MilightZone(MilightZoneType.Rgbw, 3),
                new MilightZone(MilightZoneType.Rgbw, 4)
            };

            _zoneCommands = new SortedDictionary<ILightingZone, ZoneCommandInfo>
            {
                { _zones[0 + 0], new ZoneCommandInfo(0x35, 0x39, 0xB9, 0xB5, null) },
                { _zones[0 + 1], new ZoneCommandInfo(0x38, 0x3B, 0xBB, 0xB8, null) },
                { _zones[0 + 2], new ZoneCommandInfo(0x3D, 0x33, 0xB3, 0xBD, null) },
                { _zones[0 + 3], new ZoneCommandInfo(0x37, 0x3A, 0xBA, 0xB7, null) },
                { _zones[0 + 4], new ZoneCommandInfo(0x32, 0x36, 0xB6, 0xB2, null) },
                { _zones[4 + 1], new ZoneCommandInfo(0x22, 0x21, null, null, null) },
                { _zones[6 + 0], new ZoneCommandInfo(0x42, 0x41, null, null, 0xC2) },
                { _zones[6 + 1], new ZoneCommandInfo(0x45, 0x46, null, null, 0xC5) },
                { _zones[6 + 2], new ZoneCommandInfo(0x47, 0x48, null, null, 0xC7) },
                { _zones[6 + 3], new ZoneCommandInfo(0x49, 0x4A, null, null, 0xC9) },
                { _zones[6 + 4], new ZoneCommandInfo(0x4B, 0x4C, null, null, 0xCB) }
            };
        }

        public Controller(INetworkService networkService)
        {
            _networkService = networkService;
        }

        public IPEndpoint RemoteEndpoint { get; set; }

        public async Task InitializeAsync()
        {
            _udpClient = _networkService.CreateUdpClient(8899);
            await _udpClient.ConnectAsync(RemoteEndpoint.Address, RemoteEndpoint.Port);
        }

        public Task TurnOnAsync(ILightingZone zone)
        {
            var cmds = GetZoneCommands(zone);
            return SendAsync(cmds.On);
        }

        public Task TurnOffAsync(ILightingZone zone)
        {
            var cmds = GetZoneCommands(zone);
            return SendAsync(cmds.Off);
        }

        public async Task SetBrightnessAsync(ILightingZone zone, Brightness brightness)
        {
            await TurnOnAsync(zone);
            await SendAsync(0x4E, ConvertBrightness(brightness));
        }

        public async Task SetColorAsync(ILightingZone zone, Color color)
        {
            if (color == Color.White)
            {
                var cmds = GetZoneCommands(zone);
                await SendAsync(cmds.White.Value);
            }
            else
            {
                await TurnOnAsync(zone);
                await SendAsync(0x40, ConvertColor(color));
            }
        }

        private Task SendAsync(byte[] data)
        {
            return _udpClient.SendAsync(data);
        }

        private async Task SendAsync(byte command, byte data = 0x00)
        {
            for (int i = 0; i < 5; i++)
            {
                await _udpClient.SendAsync(new byte[] { command, data, 0x55 });
            }
            await Task.Delay(100);
        }

        private ZoneCommandInfo GetZoneCommands(ILightingZone zone)
        {
            return _zoneCommands[zone];
        }

        public Task<IList<ILightingZone>> GetZonesAsync()
        {
            return Task.FromResult<IList<ILightingZone>>(new ReadOnlyCollection<ILightingZone>(_zones));
        }

        private byte ConvertBrightness(Brightness brightness)
        {
            switch (brightness.Type)
            {
                case BrightnessType.Absolute:
                    return (byte)brightness.Value;
                case BrightnessType.Percentage:
                    return (byte)Math.Max(2, Math.Round(brightness.Value * 0.27m));
                default:
                    throw new NotSupportedException();
            }
        }

        private byte ConvertColor(Color color)
        {
            var hsl = RgbToHsl(color.Red, color.Green, color.Blue);
            return HslToMilightColor(hsl);
        }

        private decimal[] RgbToHsl(int red, int green, int blue)
        {
            decimal r = red / 255m;
            decimal g = green / 255m;
            decimal b = blue / 255m;

            decimal max = new[] { r, g, b }.Max();
            decimal min = new[] { r, g, b }.Min();

            decimal h = 0;
            decimal s = 0;
            decimal l = (max + min) / 2;

            decimal delta = max - min;

            if (delta > 0)
            {
                s = delta / (1 - Math.Abs(2 * l - 1));

                if (r == max)
                {
                    h = 60 * (((g - b) / delta) % 6m);
                    if (b > g) h += 360;
                }
                else if (g == max)
                {
                     h = 60 * ((b - r) / delta + 2);
                }
                else if (b == max)
                {
                     h = 60 * ((r - g) / delta + 4);
                }
            }

            return new[] { h, s, l };
        }

        private byte HslToMilightColor(decimal[] hsl)
        {
             return (byte)((256 + 176 - (int)(hsl[0] / 360m * 255m)) % 256);
        }
    }
}
