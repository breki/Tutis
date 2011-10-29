using System.Collections.Generic;

namespace GisExperiments.ContoursLabeling
{
    public interface IContoursForElevation
    {
        IEnumerable<IContourLine> EnumerateLines();
    }
}