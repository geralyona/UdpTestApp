using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    internal class Sender
    {
        private readonly RateGenerator _generator;
        private readonly IPAddress _multiCastGroup;
        private readonly int _port;

        public Sender(int port, IPAddress multiCastGroup, RateGenerator generator)
        {
            if (port < 0) throw new ArgumentOutOfRangeException(nameof(port));
            if (port > 65535) throw new ArgumentOutOfRangeException(nameof(port));
            _port = port;
            _multiCastGroup = multiCastGroup ?? throw new ArgumentNullException(nameof(multiCastGroup));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
        }

        public void Start()
        {
            while (true)
            {
                var rate = _generator.Generate();
                var data = BitConverter.GetBytes(rate.Number).Concat(BitConverter.GetBytes(rate.Counter)).ToArray();
                using (var udpClient = new UdpClient())
                {
                    var ipEndPoint = new IPEndPoint(_multiCastGroup, _port);
                    udpClient.Send(data, data.Length, ipEndPoint);
                    udpClient.Close();
                }

                if (rate.Counter % 10000 == 0)
                    Console.WriteLine($"{rate.Counter} sent");
            }
        }
    }
}
