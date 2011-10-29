using Brejc.Geometry;

namespace GisExperiments.ContoursLabeling
{
    public interface IContourLine
    {
        GraphicPolylineAnalysis PolylineAnalysis { get; }
        float Length { get; set; }
        bool IsClosed { get; }
    }
}