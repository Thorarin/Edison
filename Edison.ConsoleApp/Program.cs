using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Edison.ConsoleApp.Net;
using Edison.Plugin.Common.Lighting;
using Edison.Plugin.Common.Net;
using Edison.Plugin.MiLight;
using Microsoft.Practices.Unity;

namespace Edison.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            CommandLine.Parser.Default.ParseArguments(args, options);

            Console.WriteLine(String.Join(" ", args));

            var container = new UnityContainer();
            container.RegisterType<INetworkService, NetworkService>();

            var plugin = container.Resolve<MiLightPlugin>();
            var task = ExecuteOptions(plugin, options);
            //var task =  Test(plugin);
            task.Wait();

            Console.WriteLine("Done");

            //Thread.Sleep(1000);
        }

        private static async Task ExecuteOptions(MiLightPlugin plugin, Options options)
        {
            var discoveredBridges = await plugin.DiscoverAsync(TimeSpan.FromMilliseconds(500));
            var controller = await plugin.GetControllerAsync(discoveredBridges.First());
            var zones = await controller.GetZonesAsync();

            var zone = zones.SingleOrDefault(z => z.Name == options.Zone);
            if (zone == null)
            {
                Console.WriteLine("Zone not found.");
            }
            
            if (options.Color != null)
            {
                Color color = ParseColor(options.Color);
                await controller.SetColorAsync(zone, color);
            }
            else if (options.White)
            {
                await controller.SetColorAsync(zone, Color.White);
            }

            if (options.Brightness != null)
            {
                var brightness = ParseBrightness(options.Brightness);
                if (brightness.Value > 0)
                {
                    await controller.SetBrightnessAsync(zone, brightness);
                }
                else
                {
                    await controller.TurnOffAsync(zone);
                }
            }
        }

        private static Color ParseColor(string str)
        {
            if (str.StartsWith("#") && str.Length == 7)
            {
                return new Color(
                    Byte.Parse(str.Substring(1, 2), NumberStyles.HexNumber),
                    Byte.Parse(str.Substring(3, 2), NumberStyles.HexNumber),
                    Byte.Parse(str.Substring(5, 2), NumberStyles.HexNumber));
            }

            throw new NotSupportedException();
        }

        private static Brightness ParseBrightness(string str)
        {
            var match = Regex.Match(str, @"^(?<Percentage>\d{1,3})%$");
            if (match.Success)
            {
                return Brightness.FromPercentage(Int32.Parse(match.Groups["Percentage"].Value));
            }

            int value;
            if (Int32.TryParse(str, out value))
            {
                return Brightness.FromValue(value);
            }

            throw new FormatException("Unrecognized brightness value.");
        }
    
    }
}
