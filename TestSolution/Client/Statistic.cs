using System;

namespace Client
{
    public struct Statistic
    {
        public Statistic(double arithmeticMean, double standardDeviation, double median, int[] modes, long count)
        {
            ArithmeticMean = arithmeticMean;
            StandardDeviation = standardDeviation;
            Median = median;
            Modes = modes ?? throw new ArgumentNullException(nameof(modes));
            Count = count;
        }

        public double ArithmeticMean { get; }

        public double StandardDeviation { get; }

        public double Median { get; }

        public int[] Modes { get; }

        public long Count { get; }

        public override string ToString()
        {
            var modesString = Modes == null
                ? "<null>"
                : string.Join(", ", Modes);
            return $"Count: {Count}; Mean: {ArithmeticMean:#.000};  Median: {Median}; Deviation: {StandardDeviation:#.000}; Modes: {modesString}";
        }
    }
}
