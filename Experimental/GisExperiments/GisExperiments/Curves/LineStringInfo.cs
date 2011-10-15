using System;
using System.Collections.Generic;
using Brejc.Geometry;

namespace GisExperiments.Curves
{
    public class LineStringInfo
    {
        public LineStringInfo()
        {
        }

        public LineStringInfo (IPointF2 a, IPointF2 b)
        {
            points.Add(a);
            points.Add(b);
        }

        public IList<IPointF2> Points
        {
            get { return points; }
        }

        public void AddPoint (IPointF2 point)
        {
            points.Add(point);
        }

        public IPointF2 CalculatePointAtLength (float length, out float angle)
        {
            throw new NotImplementedException ();
        }

        private List<IPointF2> points = new List<IPointF2> ();
    }
}