namespace Edison.Plugin.Common.Lighting
{
    public struct Color
    {
        public Color(byte red, byte green, byte blue) : this()
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public byte Red { get; private set; }
        public byte Green { get; private set; }
        public byte Blue { get; private set; }
    }
}
