using Brejc.Geometry;

namespace GisExperiments.ContoursLabeling
{
    public class ContourLabel
    {
        public ContourLabel (float linePosition, Point2<float> location)
        {
            this.linePosition = linePosition;
            this.location = location;
        }

        public float LinePosition
        {
            get { return linePosition; }
            set { linePosition = value; }
        }

        public Point2<float> Location
        {
            get { return location; }
        }

        public ContourLabel CloneDeep()
        {
            ContourLabel clone = new ContourLabel(linePosition, (Point2<float>)location.Clone());
            return clone;
        }

        private float linePosition;
        private readonly Point2<float> location;
    }
}