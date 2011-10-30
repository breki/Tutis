using System.Collections.Generic;

namespace GisExperiments.ContoursLabeling
{
    public interface IContoursForElevation
    {
        double Elevation { get; }
        IEnumerable<IContourLine> EnumerateLines();
    }

    public class ContoursForElevation : IContoursForElevation
    {
        public ContoursForElevation(double elevation)
        {
            this.elevation = elevation;
        }

        public double Elevation
        {
            get { return elevation; }
        }

        public void AddContourLine(ContourLine contourLine)
        {
            contourLines.Add(contourLine);
        }

        public IEnumerable<IContourLine> EnumerateLines()
        {
            return contourLines;
        }

        private readonly double elevation;
        private List<IContourLine> contourLines = new List<IContourLine>();
    }
}