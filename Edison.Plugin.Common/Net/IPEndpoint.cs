namespace Edison.Plugin.Common.Net
{
    public struct IPEndpoint
    {
        public IPEndpoint(IPAddress address, int port) : this()
        {
            Address = address;
            Port = port;
        }

        public IPAddress Address { get; private set; }

        public int Port { get; private set; }
    }
}
