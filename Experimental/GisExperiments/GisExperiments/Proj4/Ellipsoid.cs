namespace GisExperiments.Proj4
{
    public class Ellipsoid : IEllipsoid
    {
        public static Ellipsoid WithRf(string ellipsoidCode, double a, double rf, string ellipsoidName)
        {
            Ellipsoid ellipsoid = new Ellipsoid();
            ellipsoid.ellipsoidCode = ellipsoidCode;
            ellipsoid._a = a;
            ellipsoid._b = (1.0 - 1.0/rf)*a;
            ellipsoid._rf = rf;
            ellipsoid.ellipsoidName = ellipsoidName;

            return ellipsoid;
        }

        public static Ellipsoid WithAb (string ellipsoidCode, double a, double b, string ellipsoidName)
        {
            Ellipsoid ellipsoid = new Ellipsoid ();
            ellipsoid.ellipsoidCode = ellipsoidCode;
            ellipsoid._a = a;
            ellipsoid._b = b;
            ellipsoid._rf = 1 / (1 - b / a);
            ellipsoid.ellipsoidName = ellipsoidName;

            return ellipsoid;
        }

        public string EllipsoidCode
        {
            get { return ellipsoidCode; }
        }

        public double a
        {
            get { return _a; }
        }

        public double b
        {
            get { return _b; }
        }

        public double rf
        {
            get { return _rf; }
        }

        public string EllipsoidName
        {
            get { return ellipsoidName; }
        }

        private string ellipsoidCode;
        private double _a;
        private double _b;
        private double _rf;
        private string ellipsoidName;
    }
}