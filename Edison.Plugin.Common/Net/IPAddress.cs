using System;

namespace Edison.Plugin.Common.Net
{
    public class IPAddress : IEquatable<IPAddress>
    {
        private readonly string _address;

        static IPAddress()
        {
            Broadcast = new IPAddress("255.255.255.255");
        }

        private IPAddress(string address)
        {
            _address = address;
        }

        public static readonly IPAddress Broadcast;

        public static IPAddress Parse(string address)
        {
            return new IPAddress(address);
        }

        public static implicit operator IPAddress(string address)
        {
            return new IPAddress(address);
        }

        public static implicit operator string(IPAddress address)
        {
            return address._address;
        }

        public bool Equals(IPAddress other)
        {
            return other != null && other._address == _address;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IPAddress);
        }

        public override int GetHashCode()
        {
            return _address.GetHashCode();
        }

        public override string ToString()
        {
            return _address;
        }
    }
}
