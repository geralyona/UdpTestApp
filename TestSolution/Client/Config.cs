using System;
using System.Xml;

namespace Client
{
    internal class Config
    {
        public Config(string address = "225.0.0.1", int port = 8088, int delay = 5)
        {
            Address = address;
            Port = port;
            Delay = delay;
        }

        public int Port { get; }
        public string Address { get; }
        public int Delay { get; }

        public static Config GetConfig()
        {
            var doc = new XmlDocument();
            try
            {
                doc.Load("config.xml");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new Config();
            }

            var delay = 5;
            var delayNodeList = doc.GetElementsByTagName("delay");
            if (delayNodeList.Count == 1)
            {
                var delayStr = delayNodeList[0].InnerText;
                if (int.TryParse(delayStr, out var value) && value > 0)
                    delay = value;
            }

            var port = 8088;
            var portNodeList = doc.GetElementsByTagName("port");
            if (portNodeList.Count == 1)
            {
                var portStr = portNodeList[0].InnerText;
                if (int.TryParse(portStr, out var value) && value >= 0)
                    port = value;
            }


            var address = "225.0.0.1";
            var addressNodeList = doc.GetElementsByTagName("address");
            if (addressNodeList.Count == 1)
                address = addressNodeList[0].InnerText;

            return new Config(address, port, delay);
        }
    }
}
