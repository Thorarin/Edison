using Edison.Plugin.Common.Net;

namespace Edison.Plugin.MiLight
{
    public class WifiBridgeInfo
    {
        public WifiBridgeInfo(IPAddress address, string physicalAddress)
        {
            Address = address;
            PhysicalAddress = physicalAddress;
        }

        public IPAddress Address { get; private set; }

        public string PhysicalAddress { get; private set; }
    }
}
