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

            Type type = projections[projectionCode];
            return (IProjection)Activator.CreateInstance(type);
        }

        public static void AddProjection<TProjection> (string projectionCode) where TProjection : IProjection
        {
            projections.Add (projectionCode, typeof(TProjection));
        }

        static Projections ()
        {
            AddProjection<LongLatProjection>("longlat");
            AddProjection<TransverseMercatorProjection>("tmerc");
        }

        private static Dictionary<string, Type> projections = new Dictionary<string, Type> ();
    }
}