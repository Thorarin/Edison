using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Edison.ConsoleApp.Net
{
    internal class Test : System.Net.Sockets.UdpClient
    {
        public Test()
        {
            SetSocketOptions();
        }

        public Test(IPEndPoint ipLocalEndPoint) : base(ipLocalEndPoint)
        {
            SetSocketOptions();
        }

        public void SetSocketOptions()
        {
            //Calls the protected Client property belonging to the UdpClient base class.
            var s = Client;
            //Uses the Socket returned by Client to set an option that is not available using UdpClient.
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, 1);            
        }
    }
}
