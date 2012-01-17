using System;

namespace GisExperiments.Proj4
{
    public class TransverseMercatorProjection : IProjection
    {
        public string ProjectionCode
        {
            get { return "tmerc"; }
        }

        public string ProjectionName
        {
            get { return "Transverse Mercator"; }
        }

        public void inverse (double?[] coords)
        {
            throw new System.NotImplementedException();
        }

        public void forward (double?[] coords)
        {
            double lon = coords[0].Value;
            double lat = coords[1].Value;

            var delta_lon = ProjectionUtils.adjust_lon (lon - this.long0); // Delta longitude
            var con;    // cone constant
            var x, y;
            var sin_phi = Math.Sin (lat);
            var cos_phi = Math.Cos (lat);

            if (this.sphere)
            {  /* spherical form */
                var b = cos_phi * Math.Sin (delta_lon);
                if ((Math.Abs (Math.Abs (b) - 1.0)) < .0000000001)
                {
                    Proj4js.reportError ("tmerc:forward: Point projects into infinity");
                    return (93);
                }
                else
                {
                    x = .5 * this.a * this.k0 * Math.log ((1.0 + b) / (1.0 - b));
                    con = Math.Acos (cos_phi * Math.Cos (delta_lon) / Math.Sqrt (1.0 - b * b));
                    if (lat < 0) con = -con;
                    y = this.a * this.k0 * (con - this.lat0);
                }
            }
            else
            {
                var al = cos_phi * delta_lon;
                var als = Math.Pow (al, 2);
                var c = this.ep2 * Math.Pow (cos_phi, 2);
                var tq = Math.Tan (lat);
                var t = Math.Pow (tq, 2);
                con = 1.0 - this.es * Math.Pow (sin_phi, 2);
                var n = this.a / Math.Sqrt (con);
                var ml = this.a * Proj4js.common.mlfn (this.e0, this.e1, this.e2, this.e3, lat);

                x = this.k0 * n * al * (1.0 + als / 6.0 * (1.0 - t + c + als / 20.0 * (5.0 - 18.0 * t + Math.Pow (t, 2) + 72.0 * c - 58.0 * this.ep2))) + this.x0;
                y = this.k0 * (ml - this.ml0 + n * tq * (als * (0.5 + als / 24.0 * (5.0 - t + 9.0 * c + 4.0 * Math.Pow (c, 2) + als / 30.0 * (61.0 - 58.0 * t + Math.pow (t, 2) + 600.0 * c - 330.0 * this.ep2))))) + this.y0;

            }

            p.x = x; p.y = y;
        }
    }
}