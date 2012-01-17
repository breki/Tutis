using System;
using System.Collections.Generic;
using System.Globalization;

namespace GisExperiments.Proj4
{
    public static class Ellipsoids
    {
        public static IEllipsoid GetEllipsoid (string ellipsoidCode)
        {
            if (!ellipsoids.ContainsKey (ellipsoidCode))
            {
                string message = string.Format (CultureInfo.InvariantCulture, "Ellipsoid '{0}' is not supported.", ellipsoidCode);
                throw new InvalidOperationException (message);
            }

            return ellipsoids[ellipsoidCode];
        }

        public static void AddEllipsoid (IEllipsoid ellipsoid)
        {
            ellipsoids.Add(ellipsoid.EllipsoidCode, ellipsoid);
        }

        static Ellipsoids()
        {
            AddEllipsoid (Ellipsoid.WithRf ("WGS84", 6378137.0, 298.257223563, "WGS 84"));
            AddEllipsoid (Ellipsoid.WithAb ("airy", 6377563.396, 6356256.910, "Airy 1830"));
        }

        private static Dictionary<string, IEllipsoid> ellipsoids = new Dictionary<string, IEllipsoid> ();
    }
}