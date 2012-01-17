using Brejc.Geometry;

namespace GisExperiments.Proj4
{
    public interface IProjection
    {
        string ProjectionCode { get; }
        string ProjectionName { get; }

        void inverse (double?[] coords);
        void forward (double?[] coords);
    }
}