using System;

namespace Server
{
    public class RateGenerator
    {
        private readonly int _maxNumber;
        private readonly int _minNumber;
        private readonly Random _random;
        private int _counter;

        public RateGenerator(int minNumber, int maxNumber)
        {
            _minNumber = minNumber;
            _maxNumber = maxNumber;
            _random = new Random();
        }

        public Rate Generate()
        {
            return new Rate(_random.Next(_minNumber, _maxNumber), unchecked(++_counter));
        }
    }
}
