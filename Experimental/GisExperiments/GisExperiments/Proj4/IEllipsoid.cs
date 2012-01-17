namespace GisExperiments.Proj4
{
    public interface IEllipsoid
    {
        string EllipsoidCode { get; }
        double a { get; }
        double b { get; }
        double rf { get; }
        string EllipsoidName { get; }
        bool IsSphere { get; }
    }
}