namespace Edison.Plugin.Common.Net
{
    public class UdpReceiveResult
    {
        public UdpReceiveResult(byte[] data, IPEndpoint remoteEndpoint)
        {
            Data = data;
            RemoteEndpoint = remoteEndpoint;
        }

        public byte[] Data { get; private set; }

        public IPEndpoint RemoteEndpoint { get; private set; }
    }
}
