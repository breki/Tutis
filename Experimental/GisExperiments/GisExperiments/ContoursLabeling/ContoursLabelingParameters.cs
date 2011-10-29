using Brejc.Geometry;

namespace GisExperiments.ContoursLabeling
{
    public class ContoursLabelingParameters
    {
        public double ElevationInterval
        {
            get { return elevationInterval; }
            set { elevationInterval = value; }
        }

        public float LabelCoverageRange
        {
            get { return labelCoverageRange; }
            set { labelCoverageRange = value; }
        }

        public double MinimumContourLength
        {
            get { return minimumContourLength; }
            set { minimumContourLength = value; }
        }

        public double MinimumSameLabelDistance
        {
            get { return minimumSameLabelDistance; }
            set { minimumSameLabelDistance = value; }
        }

        public Bounds2 ProcessBounds
        {
            get { return processBounds; }
            set { processBounds = value; }
        }

        private double elevationInterval;
        private double minimumContourLength = 200;
        private double minimumSameLabelDistance = 200;
        private float labelCoverageRange = 400;
        private Bounds2 processBounds;
    }
}