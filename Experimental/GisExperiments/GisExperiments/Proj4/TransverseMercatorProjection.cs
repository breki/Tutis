using System;

namespace GisExperiments.Proj4
{
    public class TransverseMercatorProjection : SrsBase
    {
        public override string ProjectionCode
        {
            get { return "tmerc"; }
        }

        public override string ProjectionName
        {
            get { return "Transverse Mercator"; }
        }

        public override void inverse (double?[] coords)
        {
            throw new System.NotImplementedException();
        }

        public override void forward (double?[] coords)
        {
            double lon = coords[0].Value;
            double lat = coords[1].Value;

            double delta_lon = ProjectionUtils.adjust_lon (lon - long0); // Delta longitude
            double con;    // cone constant
            double x, y;
            double sin_phi = Math.Sin (lat);
            double cos_phi = Math.Cos (lat);

            if (Ellipsoid.IsSphere)
            {  /* spherical form */
                double b = cos_phi * Math.Sin (delta_lon);
                if ((Math.Abs (Math.Abs (b) - 1.0)) < .0000000001)
                    throw new InvalidOperationException("tmerc:forward: Point projects into infinity");

                x = .5 * Ellipsoid.a * k0.Value * Math.Log ((1.0 + b) / (1.0 - b));
                con = Math.Acos (cos_phi * Math.Cos (delta_lon) / Math.Sqrt (1.0 - b * b));
                if (lat < 0) con = -con;
                y = Ellipsoid.a * k0.Value * (con - lat0);
            }
            else
            {
                var al = cos_phi * delta_lon;
                var als = Math.Pow (al, 2);
                var c = Datum.ep2 * Math.Pow (cos_phi, 2);
                var tq = Math.Tan (lat);
                var t = Math.Pow (tq, 2);
                con = 1.0 - Datum.es * Math.Pow (sin_phi, 2);
                var n = Ellipsoid.a / Math.Sqrt (con);
                var ml = Ellipsoid.a * ProjectionUtils.mlfn (e0, e1, e2, e3, lat);

                x = k0.Value * n * al * (1.0 + als / 6.0 * (1.0 - t + c + als / 20.0 * (5.0 - 18.0 * t + Math.Pow (t, 2) + 72.0 * c - 58.0 * Datum.ep2))) + x0;
                y = k0.Value * (ml - ml0 + n * tq * (als * (0.5 + als / 24.0 * (5.0 - t + 9.0 * c + 4.0 * Math.Pow (c, 2) + als / 30.0 * (61.0 - 58.0 * t + Math.Pow (t, 2) + 600.0 * c - 330.0 * Datum.ep2))))) + y0;
            }

            coords[0] = x; 
            coords[1] = y;
        }

        public override void Initialize ()
        {
            base.Initialize ();

            e0 = ProjectionUtils.e0fn (Datum.es);
            e1 = ProjectionUtils.e1fn (Datum.es);
            e2 = ProjectionUtils.e2fn (Datum.es);
            e3 = ProjectionUtils.e3fn (Datum.es);
            ml0 = Ellipsoid.a * ProjectionUtils.mlfn (e0, e1, e2, e3, lat0);
        }

        private double e0, e1, e2, e3;
        private double ml0;
    }
}