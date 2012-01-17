using System;
using Brejc.Geometry;

namespace GisExperiments.Proj4
{
    public static class ProjectionUtils
    {
        public static double adjust_lon (double x)
        {
            return (Math.Abs (x) < Math.PI) ? x : (x - (Math.Sign (x) * MathExt.C2PI));
        }

        public static double e0fn (double x)
        {
            return (1.0 - 0.25 * x * (1.0 + x / 16.0 * (3.0 + 1.25 * x)));
        }

        public static double e1fn (double x)
        {
            return (0.375 * x * (1.0 + 0.25 * x * (1.0 + 0.46875 * x)));
        }

        public static double e2fn (double x)
        {
            return (0.05859375 * x * x * (1.0 + 0.75 * x));
        }

        public static double e3fn (double x)
        {
            return (x * x * x * (35.0 / 3072.0));
        }

        public static double mlfn (double e0, double e1, double e2, double e3, double phi)
        {
            return (e0 * phi - e1 * Math.Sin (2.0 * phi) + e2 * Math.Sin (4.0 * phi) - e3 * Math.Sin (6.0 * phi));
        }
    }
}