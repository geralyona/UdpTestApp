using System;
using System.Net;
using System.Threading;

namespace Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            long missingPackages = 0;
            var lastCounter = 0;
            var firstPackage = true;
            var statisticCalculator = new StatisticCalculator();
            var processor = new Processor();

            void Handler(int number, int counter)
            {
                if (firstPackage)
                {
                    firstPackage = false;
                    lastCounter = unchecked(counter - 1);
                }

                var missing = unchecked(counter - lastCounter - 1);
                if (missing < 0) missing = 0;

                missingPackages += missing;
                lastCounter = counter;
                processor.Enqueue(() => statisticCalculator.AddNumber(number));
            }

            var config = Config.GetConfig();

            var receiver = new Receiver(config.Port, IPAddress.Parse(config.Address), Handler);

            new Thread(processor.Start).Start();
            new Thread(receiver.Start).Start();

            Console.WriteLine($@"Enter d to use standard delay ({config.Delay}s)
Enter positive number to delay for custom amount of seconds
Press enter to display statistic");
            while (true)
            {
                var s = Console.ReadLine();
                if (s == "d")
                {
                    receiver.Delay(config.Delay);
                    continue;
                }

                if (int.TryParse(s, out var delaySeconds) && delaySeconds > 0)
                {
                    receiver.Delay(delaySeconds);
                    continue;
                }

                var missingPackagesMessage = missingPackages == 0 ? string.Empty : $"\r\n {missingPackages} packages missing";
                processor.Enqueue(() => Console.WriteLine($"{statisticCalculator.Statistic}{missingPackagesMessage}"));
            }
        }
    }
}
