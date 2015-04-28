using System;
using System.Globalization;
using Edison.Plugin.Common.Lighting;

namespace Edison.Plugin.MiLight
{
    internal class MilightZone : ILightingZone, IComparable<MilightZone>, IComparable
    {
        public MilightZone(MilightZoneType type, int number)
        {
            Type = type;
            Number = number;
        }

        public int Number { get; set; }

        public MilightZoneType Type { get; set; }

        public string Name
        {
            get
            {
                switch (Type)
                {
                    case MilightZoneType.White:
                        return "White " + (Number == 0 ? "All" : Number.ToString(CultureInfo.InvariantCulture));
                    case MilightZoneType.Rgb:
                        return "RGB " + (Number == 0 ? "All" : Number.ToString(CultureInfo.InvariantCulture));
                    case MilightZoneType.Rgbw:
                        return "RGBW " + (Number == 0 ? "All" : Number.ToString(CultureInfo.InvariantCulture));
                    default:
                        return "Unknown";
                }
            }
        }

        public int CompareTo(MilightZone other)
        {
            if (other == null) return -1;
            if (Type != other.Type) return (int)Type - (int)other.Type;
            return Number - other.Number;
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as MilightZone);
        }
    }
}
