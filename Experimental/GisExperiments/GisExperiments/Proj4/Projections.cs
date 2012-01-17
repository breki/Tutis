using System;
using System.Collections.Generic;
using System.Globalization;

namespace GisExperiments.Proj4
{
    public static class Projections
    {
        public static IProjection GetProjection (string projectionCode)
        {
            if (!projections.ContainsKey (projectionCode))
            {
                string message = string.Format (CultureInfo.InvariantCulture, "Projection '{0}' is not supported.", projectionCode);
                throw new InvalidOperationException (message);
            }

            return projections[projectionCode];
        }

        public static void AddProjection (IProjection projection)
        {
            projections.Add (projection.ProjectionCode, projection);
        }

        static Projections ()
        {
            AddProjection(new LongLatProjection());
            AddProjection(new TransverseMercatorProjection());
        }

        private static Dictionary<string, IProjection> projections = new Dictionary<string, IProjection> ();
    }
}