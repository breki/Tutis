using System;

namespace HomeSpaceOrganizer
{
    public class Randomizer : IRandomizer
    {
        public Randomizer()
        {
            rnd = new Random();
        }

        public Randomizer(int seed)
        {
            Reset(seed);
        }

        public int Next(int maxValue)
        {
            return rnd.Next(maxValue);
        }

        public double NextDouble()
        {
            return rnd.NextDouble();
        }

        public void Reset(int seed)
        {
            rnd = new Random(seed);
        }

        private Random rnd;
    }
}