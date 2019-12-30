namespace Server
{
    public class Rate
    {
        public Rate(int number, int counter)
        {
            Number = number;
            Counter = counter;
        }

        public int Number { get; }

        public int Counter { get; }
    }
}
