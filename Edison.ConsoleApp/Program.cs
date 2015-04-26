using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Edison.ConsoleApp.Net;
using Edison.Plugin.Common.Net;
using Edison.Plugin.MiLight;
using Microsoft.Practices.Unity;

namespace Edison.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new UnityContainer();
            container.RegisterType<INetworkService, NetworkService>();

            var plugin = container.Resolve<MiLightPlugin>();

            var task = plugin.DiscoverAsync(TimeSpan.FromMilliseconds(500));

            //var task = ColorAsync(plugin, 10);
            task.Wait();

            Thread.Sleep(10000);
        }

        private static async Task BlinkAsync(MiLightPlugin plugin, int loops)
        {
            for (int i = 0; i < loops; i++)
            {
                await plugin.TurnOffAsync();
                await Task.Delay(100);
                await plugin.TurnOnAsync();
                await Task.Delay(100);
            }
        }

        private static async Task FadeAsync(MiLightPlugin plugin, int loops)
        {
            for (int i = 0; i < loops; i++)
            {
                for (int j = 2; j <= 0x1b; j++)
                {
                    await plugin.SetBrightness(j);
                    await Task.Delay(100);
                }
                for (int j = 0x1a; j >= 2; j--)
                {
                    await plugin.SetBrightness(j);
                    await Task.Delay(100);
                }
            }
        }

        private static async Task ColorAsync(MiLightPlugin plugin, int loops)
        {
            for (int i = 0; i < loops; i++)
            {
                for (int j = 0; j <= 0xff; j++)
                {
                    await plugin.SetColor(j);
                    await Task.Delay(100);
                }
            }
        }
    
    }
}
