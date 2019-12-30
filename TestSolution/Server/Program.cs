using System;
using System.Net;
using System.Threading;

namespace Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = Config.GetConfig();
            var sender = new Sender(config.Port, IPAddress.Parse(config.Address),
                new RateGenerator(config.MinRange, config.MaxRange));
            var thread = new Thread(sender.Start);
            thread.Start();

            Console.ReadLine();
            thread.Abort();
        }
    }
}
