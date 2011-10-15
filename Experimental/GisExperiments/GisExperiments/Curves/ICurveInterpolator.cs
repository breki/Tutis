using System.Collections.Generic;
using Brejc.Geometry;

namespace GisExperiments.Curves
{
    public interface ICurveInterpolator
    {
        LineStringInfo InterpolateCurve (IList<IPointF2> points, CurveInterpolationParameters parameters);
    }
}