using System;
using System.Collections.Generic;
using System.Linq;
using MoreComplexDataStructures;

namespace Client
{
    internal class StatisticCalculator
    {
        private readonly Dictionary<int, ulong> _frequencies = new Dictionary<int, ulong>();
        private readonly MaxHeap<int> _lowerHalf = new MaxHeap<int>();
        private readonly ulong _maxFrequency = 0;
        private readonly MinHeap<int> _upperHalf = new MinHeap<int>();

        private long _count;
        private double _currentArithmeticMean;

        private ulong _lowerHalfWeightedCount;

        private int[] _modes = Array.Empty<int>();
        private double _previousArithmeticMean;

        private double _sumOfSquaredDeviations;
        private ulong _upperHalfWeightedCount;

        public Statistic Statistic { get; private set; }

        public void AddNumber(int newNumber)
        {
            _count++;
            var arithmeticMean = CalculateArithmeticMean(newNumber, out var diffArithmeticMean);
            // should be called after CalculateArithmeticMean
            var deviation = CalculateStandardDeviation(newNumber, diffArithmeticMean, arithmeticMean);
            var modes = CalculateModes(newNumber);
            // should be called after CalculateModes
            var median = CalculateMedian(newNumber);

            Statistic = new Statistic(arithmeticMean, deviation, median, modes, _count);
        }

        private int[] CalculateModes(int newNumber)
        {
            if (_frequencies.TryGetValue(newNumber, out var frequency))
            {
                _frequencies[newNumber] = ++frequency;
            }
            else
            {
                frequency = 1;
                _frequencies.Add(newNumber, frequency);
            }

            if (frequency == _maxFrequency)
            {
                _modes = _modes.Concat(new[] {newNumber}).ToArray();
            }
            else if (frequency > _maxFrequency)
            {
                if (_modes == Array.Empty<int>() || _modes.Length > 1)
                    _modes = new[] {newNumber};
                else
                    _modes[0] = newNumber;
            }

            return _modes;
        }

        private double CalculateMedian(int newNumber)
        {
            BalanceMaxMinHalf(newNumber);

            if (_lowerHalfWeightedCount > _upperHalfWeightedCount)
                return _lowerHalf.Peek();
            return (double)(_lowerHalf.Peek() + _upperHalf.Peek()) / 2;
        }

        /// <summary>
        ///     This method balances heaps to preserve invariant:
        ///     _lowerHalfWeightedCount >= _upperHalfWeightedCount &&
        ///     (_lowerHalfWeightedCount - lowerPeakFrequency) less then (_upperHalfWeightedCount + lowerPeakFrequency)
        /// </summary>
        /// <param name="newNumber"></param>
        private void BalanceMaxMinHalf(int newNumber)
        {
            if (_lowerHalf.Count == 0)
            {
                _lowerHalf.Insert(newNumber);
                _lowerHalfWeightedCount = 1;
                return;
            }

            if (newNumber <= _lowerHalf.Peek())
            {
                _lowerHalfWeightedCount++;
                if (_frequencies[newNumber] == 1)
                    _lowerHalf.Insert(newNumber);
            }
            else
            {
                _upperHalfWeightedCount++;
                if (_frequencies[newNumber] == 1)
                    _upperHalf.Insert(newNumber);
            }

            if (_upperHalfWeightedCount > _lowerHalfWeightedCount)
            {
                var number = _upperHalf.ExtractMin();
                var numberFrequency = _frequencies[number];

                _lowerHalf.Insert(number);
                _lowerHalfWeightedCount += numberFrequency;
                _upperHalfWeightedCount -= numberFrequency;
            }
            else if (_lowerHalfWeightedCount > _upperHalfWeightedCount)
            {
                var lowerPeakFrequency = _frequencies[_lowerHalf.Peek()];
                if (_lowerHalfWeightedCount - lowerPeakFrequency >= _upperHalfWeightedCount + lowerPeakFrequency)
                {
                    var number = _lowerHalf.ExtractMax();
                    _upperHalf.Insert(number);
                    _lowerHalfWeightedCount -= lowerPeakFrequency;
                    _upperHalfWeightedCount += lowerPeakFrequency;
                }
            }
        }

        /// <summary>
        ///     sum of all number equals sum of all numbers without new number plus new number:
        ///     sum from i = 0 to i = count == N_1 + N_2 + ... + N_(count-1) + N_count ==
        ///     == _previousArithmeticMean * (count - 1) + N_count
        ///     (  _previousArithmeticMean * (count - 1) + N_count  ) / count ==
        ///     _previousArithmeticMean + ( N_count - _previousArithmeticMean) / count
        /// </summary>
        /// <param name="newNumber"></param>
        /// <param name="diffArithmeticMean"></param>
        private double CalculateArithmeticMean(int newNumber, out double diffArithmeticMean)
        {
            _previousArithmeticMean = _currentArithmeticMean;
            diffArithmeticMean = (newNumber - _previousArithmeticMean) / _count;

            _currentArithmeticMean += diffArithmeticMean;
            return _currentArithmeticMean;
        }

        /// <summary>
        ///     call after CalculateArithmeticMean
        ///     sum of squared deviations equals sum of all squared deviations without squared deviation of new number
        ///     plus squared deviation of new number.
        ///     number - number from previous selection
        ///     squared deviation of new number == (newNumber - currentArithmeticMean) ^ 2
        ///     sum of ( number - _previousArithmeticMean) == sum of(number) - (_count -1)*_previousArithmeticMean == 0
        ///     sum of all squared deviations without squared deviation of new number ==
        ///     sum of (( number - currentArithmeticMean)^2 ) ==
        ///     sum of (( number - _previousArithmeticMean - diffArithmeticMean)^2) ==
        ///     sum of (( number - _previousArithmeticMean)^2 - 2*diffArithmeticMean*( number - _previousArithmeticMean) +
        ///     diffArithmeticMean^2) ==
        ///     sum of (( number - _previousArithmeticMean)^2) + (_count-1)*diffArithmeticMean^2
        ///     -2*diffArithmeticMean*( sum of ( number - _previousArithmeticMean)) ==
        ///     sum of (( number - _previousArithmeticMean)^2) + (_count-1)*diffArithmeticMean^2  ==
        ///     previous_sumOfSquaredDeviations + (_count-1)*diffArithmeticMean^2
        /// </summary>
        /// <param name="newNumber"></param>
        /// <param name="diffArithmeticMean"></param>
        /// <param name="currentArithmeticMean"></param>
        private double CalculateStandardDeviation(int newNumber, double diffArithmeticMean, double currentArithmeticMean)
        {
            if (_count == 1) return 0;

            var previousCount = _count - 1;
            _sumOfSquaredDeviations += previousCount * Math.Pow(diffArithmeticMean, 2);
            var localDiff = newNumber - currentArithmeticMean;
            _sumOfSquaredDeviations += Math.Pow(localDiff, 2);

            return Math.Sqrt(_sumOfSquaredDeviations / previousCount);
        }
    }
}
