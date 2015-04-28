using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edison.Plugin.Common.Lighting
{
    public interface ILightingController
    {
        Task<IList<ILightingZone>> GetZonesAsync();

        Task TurnOnAsync(ILightingZone zone);
        Task TurnOffAsync(ILightingZone zone);

        Task SetBrightnessAsync(ILightingZone zone, Brightness brightness);
        Task SetColorAsync(ILightingZone zone, Color color);
    }
}
