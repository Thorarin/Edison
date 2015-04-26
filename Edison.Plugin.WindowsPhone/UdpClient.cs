using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Edison.Plugin.Common.Net;

namespace Edison.Plugin.WindowsPhone
{
    public class UdpClient : IUdpClient
    {
        private readonly DatagramSocket _socket;
        private readonly AutoResetEvent _receiveEvent;
        private readonly object _receiveSyncRoot = new object();
        private readonly Queue<UdpReceiveResult> _receiveBuffer = new Queue<UdpReceiveResult>();

        public UdpClient()
        {
            _socket = new DatagramSocket();
            _receiveEvent = new AutoResetEvent(false);
            _socket.MessageReceived += OnMessageReceived;
        }

        public async Task SendAsync(byte[] datagram)
        {
            using (var dw = new DataWriter(_socket.OutputStream))
            {
                dw.WriteBytes(datagram);
                await dw.FlushAsync();
            }
        }

        public async Task ConnectAsync(string remoteHost, int remotePort)
        {
            await _socket.ConnectAsync(new HostName(remoteHost), remotePort.ToString());
        }

        public Task<UdpReceiveResult> ReceiveAsync()
        {
            lock (_receiveSyncRoot)
            {
                if (_receiveBuffer.Count > 0)
                {
                    return Task.FromResult(_receiveBuffer.Dequeue());
                }
            }

            return Task.Run(() =>
            {
                _receiveEvent.WaitOne(-1);

                lock (_receiveSyncRoot)
                {
                    if (_receiveBuffer.Count > 0)
                    {
                        return _receiveBuffer.Dequeue();
                    }
                    return _receiveBuffer.Dequeue();
                }
            });
        }

        private void OnMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            UdpReceiveResult receiveResult;

            using (var dr = args.GetDataReader())
            {
                byte[] buffer = new byte[dr.UnconsumedBufferLength];
                dr.ReadBytes(buffer);

                receiveResult = new UdpReceiveResult(
                    buffer,
                    new IPEndpoint(args.RemoteAddress.CanonicalName, Int32.Parse(args.RemotePort)));
            }

            lock (_receiveSyncRoot)
            {
                _receiveBuffer.Enqueue(receiveResult);
                _receiveEvent.Set();
            }
        }


        public bool EnableBroadcast
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool DontFragment
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public Task SendAsync(byte[] datagram, IPEndpoint remoteEndpoint)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _socket.Dispose();
        }
    }
}
