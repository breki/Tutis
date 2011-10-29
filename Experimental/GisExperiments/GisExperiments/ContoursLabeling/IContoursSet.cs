using System;
using System.Collections.Generic;

namespace GisExperiments.ContoursLabeling
{
    public interface IContoursSet
    {
        IEnumerable<IContoursForElevation> EnumerateElevations();
    }

    public class ContoursSet : IContoursSet
    {
        public IEnumerable<IContoursForElevation> EnumerateElevations()
        {
            return elevations.Values;
        }

        private Dictionary<double, IContoursForElevation> elevations = new Dictionary<double, IContoursForElevation>();
    }
}