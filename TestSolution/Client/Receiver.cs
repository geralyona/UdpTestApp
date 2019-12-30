using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    public class Receiver
    {
        private readonly Action<int, int> _handler;
        private readonly IPAddress _multiCastGroup;
        private readonly int _port;
        private IPEndPoint _ipEndPoint;
        private int _nextDelaySeconds;
        private UdpClient _udpClient;

        public Receiver(int port, IPAddress multiCastGroup, Action<int, int> handler)
        {
            if (port < 0) throw new ArgumentOutOfRangeException(nameof(port));
            if (port > 65535) throw new ArgumentOutOfRangeException(nameof(port));
            _port = port;
            _multiCastGroup = multiCastGroup ?? throw new ArgumentNullException(nameof(multiCastGroup));
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public void Start()
        {
            _udpClient = new UdpClient(_port);
            _udpClient.JoinMulticastGroup(_multiCastGroup, 5);
            while (true)
            {
                if (_nextDelaySeconds > 0)
                {
                    Thread.Sleep(1000 * _nextDelaySeconds);
                    _udpClient.Dispose();
                    _udpClient = new UdpClient(_port);
                    _udpClient.JoinMulticastGroup(_multiCastGroup, 5);
                    _nextDelaySeconds = 0;
                }

                var data = _udpClient.Receive(ref _ipEndPoint);
                var number = BitConverter.ToInt32(data, 0);
                var counter = BitConverter.ToInt32(data, 4);
                _handler(number, counter);
            }
        }

        public void Delay(int delaySeconds)
        {
            _nextDelaySeconds = delaySeconds;
        }
    }
}
