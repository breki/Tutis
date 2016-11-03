namespace HomeSpaceOrganizer
{
    public interface IRandomizer
    {
        int Next(int maxValue);
        double NextDouble();
        void Reset(int seed);
    }
}