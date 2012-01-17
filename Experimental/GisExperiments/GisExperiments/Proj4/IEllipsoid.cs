namespace GisExperiments.Proj4
{
    public interface IEllipsoid
    {
        string EllipsoidCode { get; }
        double SemimajorRadius { get; }
        double SemiminorRadius { get; }
        double ReciprocalFlattening { get; }
        string EllipsoidName { get; }
        bool IsSphere { get; }
    }
}