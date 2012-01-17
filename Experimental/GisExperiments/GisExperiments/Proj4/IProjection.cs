namespace GisExperiments.Proj4
{
    public interface IProjection
    {
        string ProjectionCode { get; }
        string ProjectionName { get; }

        void Inverse (double?[] coords);
        void Forward (double?[] coords);
    }
}