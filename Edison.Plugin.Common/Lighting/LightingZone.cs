namespace Edison.Plugin.Common.Lighting
{
    public struct LightingZone
    {
        public LightingZone(int id) : this()
        {
            Id = id;
        }

        public int Id { get; private set; }

        public static LightingZone All = new LightingZone();

        public static implicit operator LightingZone(int id)
        {
            return new LightingZone(id);
        }
    }
}
