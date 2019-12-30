using System;
using System.Xml;

namespace Server
{
    internal class Config
    {
        public Config(int port = 8088, int minRange = 200, int maxRange = 50000, string address = "225.0.0.1")
        {
            if (maxRange <= minRange) throw new ArgumentOutOfRangeException(nameof(maxRange));
            Port = port;
            MinRange = minRange;
            MaxRange = maxRange;
            Address = address;
        }

        public int Port { get; }
        public int MinRange { get; }
        public int MaxRange { get; }
        public string Address { get; }

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

            var port = 8088;
            var portNodeList = doc.GetElementsByTagName("port");
            if (portNodeList.Count == 1)
            {
                var portStr = portNodeList.Item(0).InnerText;
                if (int.TryParse(portStr, out var value))
                    port = value;
            }

            var minRange = 200;
            var minRangeNodeList = doc.GetElementsByTagName("min_range");
            if (minRangeNodeList.Count == 1)
            {
                var minRangeStr = minRangeNodeList.Item(0).InnerText;
                if (int.TryParse(minRangeStr, out var value))
                    minRange = value;
            }

            var maxRange = 5000;
            var maxRangeNodeList = doc.GetElementsByTagName("max_range");
            if (maxRangeNodeList.Count == 1)
            {
                var maxRangeStr = maxRangeNodeList.Item(0).InnerText;
                if (int.TryParse(maxRangeStr, out var value))
                    maxRange = value;
            }

            var address = "225.0.0.1";
            var addressNodeList = doc.GetElementsByTagName("address");
            if (addressNodeList.Count == 1) address = addressNodeList.Item(0).InnerText;

            return new Config(port, minRange, maxRange, address);
        }
    }
}
