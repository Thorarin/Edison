namespace Edison.Plugin.Common.Lighting
{
    public struct Brightness
    {
        private Brightness(BrightnessType type, int value) : this()
        {
            Type = type;
            Value = value;
        }

        public int Value { get; private set; }

        public BrightnessType Type { get; private set; }

        public static Brightness FromValue(int value)
        {
            return new Brightness(BrightnessType.Absolute, value);
        }

        public static Brightness FromPercentage(int percentage)
        {
            return new Brightness(BrightnessType.Percentage, percentage);
        }
    }
}
