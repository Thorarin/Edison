using Edison.Plugin.Common.Lighting;

namespace Edison.Plugin.MiLight
{
    public class MilightZone : ILightingZone
    {
        public int Number { get; set; }

        public MilightZoneType Type { get; set; }
    }
}
