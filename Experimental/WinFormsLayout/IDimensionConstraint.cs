namespace App
{
    public interface IDimensionConstraint
    {
        Dimension Dimension { get; }

        //void Minimize();
        //void Maximize();
        //void Optimize();
        void Validate();
    }
}