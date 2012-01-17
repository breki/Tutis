using System;
using Brejc.Geometry;

namespace GisExperiments.Proj4
{
    public static class ProjectionUtils
    {
        public static double adjust_lon (double x)
        {
            return (Math.Abs(x) < Math.PI) ? x : (x - (Math.Sign(x)*MathExt.C2PI));
        }
    }
}