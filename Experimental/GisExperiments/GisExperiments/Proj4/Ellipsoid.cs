namespace GisExperiments.Proj4
{
    public class Ellipsoid : IEllipsoid
    {
        public static Ellipsoid WithRf(string ellipsoidCode, double a, double rf, string ellipsoidName)
        {
            Ellipsoid ellipsoid = new Ellipsoid();
            ellipsoid.ellipsoidCode = ellipsoidCode;
            ellipsoid.semimajorRadius = a;
            ellipsoid.semiminorRadius = (1.0 - 1.0/rf)*a;
            ellipsoid.reciprocalFlattening = rf;
            ellipsoid.ellipsoidName = ellipsoidName;

            return ellipsoid;
        }

        public static Ellipsoid WithAb (string ellipsoidCode, double a, double b, string ellipsoidName)
        {
            Ellipsoid ellipsoid = new Ellipsoid ();
            ellipsoid.ellipsoidCode = ellipsoidCode;
            ellipsoid.semimajorRadius = a;
            ellipsoid.semiminorRadius = b;
            ellipsoid.reciprocalFlattening = 1 / (1 - b / a);
            ellipsoid.ellipsoidName = ellipsoidName;

            return ellipsoid;
        }

        public string EllipsoidCode
        {
            get { return ellipsoidCode; }
        }

        public double SemimajorRadius
        {
            get { return semimajorRadius; }
        }

        public double SemiminorRadius
        {
            get { return semiminorRadius; }
        }

        public double ReciprocalFlattening
        {
            get { return reciprocalFlattening; }
        }

        public string EllipsoidName
        {
            get { return ellipsoidName; }
        }

        public bool IsSphere
        {
            get { return semimajorRadius == semiminorRadius; }
        }

        private string ellipsoidCode;
        private double semimajorRadius;
        private double semiminorRadius;
        private double reciprocalFlattening;
        private string ellipsoidName;
    }
}