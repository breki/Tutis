using System;
using System.Collections.Generic;

namespace GisExperiments.ContoursLabeling
{
    public interface IContoursSet
    {
        void AddElevation(IContoursForElevation elevation);
        IEnumerable<IContoursForElevation> EnumerateElevations();
    }

    public class ContoursSet : IContoursSet
    {
        public void AddElevation(IContoursForElevation elevation)
        {
            elevations.Add(elevation.Elevation, elevation);
        }

        public IEnumerable<IContoursForElevation> EnumerateElevations()
        {
            return elevations.Values;
        }

        private Dictionary<double, IContoursForElevation> elevations = new Dictionary<double, IContoursForElevation>();
    }
}