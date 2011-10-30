using System;
using System.Collections.Generic;
using Brejc.Geometry;

namespace GisExperiments.ContoursLabeling
{
    public interface IContourLine
    {
        GraphicPolylineAnalysis PolylineAnalysis { get; }
        float Length { get; }
        bool IsClosed { get; }
    }

    public class ContourLine : IContourLine
    {
        public ContourLine(IEnumerable<IPointF2> points)
        {
            this.points.AddRange(points);
        }

        public GraphicPolylineAnalysis PolylineAnalysis
        {
            get
            {
                if (polylineAnalysis == null)
                {
                    float[] coords = new float[points.Count * 2];

                    for (int i = 0; i < points.Count; i++)
                    {
                        coords[i * 2] = points[i].X;
                        coords[i * 2 + 1] = points[i].Y;
                    }

                    polylineAnalysis = new GraphicPolylineAnalysis(coords);
                    polylineAnalysis.Analyze();
                }

                return polylineAnalysis;
            }
        }
        
        public float Length
        {
            get { return PolylineAnalysis.Length; }
        }
        
        public bool IsClosed
        {
            get { return points[0] == points[points.Count-1]; }
        }

        private List<IPointF2> points = new List<IPointF2>();
        private GraphicPolylineAnalysis polylineAnalysis;
    }
}