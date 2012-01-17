namespace GisExperiments.Proj4
{
    public abstract class SrsBase : ISrs
    {
        public abstract string ProjectionCode { get;  }
        public abstract string ProjectionName { get;  }

        public double alpha { get; set; }
        public IDatum Datum { get; set; }
        public string axis { get; set; }
        public IEllipsoid Ellipsoid { get; set; }
        public double? from_greenwich { get; set; }
        public double? k0 { get; set; }
        public double lat0 { get; set; }
        public double lat1 { get; set; }
        public double lat2 { get; set; }
        public double lat_ts { get; set; }
        public double long0 { get; set; }
        public double longc { get; set; }
        public string nagrids { get; set; }
        public bool R_A { get; private set; }
        public double? to_meter { get; set; }
        public string units { get; set; }
        public bool utmSouth { get; set; }
        public string title { get; set; }
        public double x0 { get; set; }
        public double y0 { get; set; }
        public int zone { get; set; }
        public abstract void inverse(double?[] coords);
        public abstract void forward(double?[] coords);

        public virtual void Initialize()
        {
            //if (nagrids == "@null")
            //    datumCode = "none";

            //if (datumCode != null && datumCode != "none")
            //{
            //    IDatum datumDef = Datums.GetDatum (datumCode);

            //    if (datumDef != null)
            //    {
            //        if (datumDef.towgs84 != null)
            //            datum_params = Proj4.Datum.ParseDatumParams (datumDef.towgs84);
            //        Ellipsoid = datumDef.Ellipsoid;
            //        datumName = datumDef.DatumName ?? datumCode;
            //    }
            //}

            //if (!a.HasValue)
            //{
            //    // do we have an ellipsoid?
            //    IEllipsoid ellipse = Ellipsoids.GetEllipsoid (ellps) ?? Ellipsoids.GetEllipsoid ("WGS84");
            //    throw new NotImplementedException();
            //    //Proj4js.extend (this, ellipse);
            //}

            //if (rf.HasValue && !b.HasValue)
            //    b = (1.0 - 1.0 / rf) * a;

            //if (rf == 0 || Math.Abs (a.Value - b.Value) < 1.0e-10)
            //{
            //    //sphere = true;
            //    b = a;
            //}

            //double a2 = Ellipsoid.a * Ellipsoid.a;          // used in geocentric
            //double b2 = Ellipsoid.b * Ellipsoid.b;          // used in geocentric
            //es = (a2 - b2) / a2;  // e ^ 2
            //e = Math.Sqrt (es);        // eccentricity
            //if (R_A)
            //{
            //    a *= 1 - es * (1.0 / 6 + es * (17.0 / 360 + es * 67.0 / 3024));
            //    a2 = a.Value * a.Value;
            //    b2 = b.Value * b.Value;
            //    es = 0;
            //}

            //ep2 = (a2 - b2) / b2; // used in geocentric
            if (!k0.HasValue)
                k0 = 1.0;    //default value
            //DGR 2010-11-12: axis
            if (axis == null)
                axis = "enu";

            //datum = new Proj4js.datum (this);
        }
    }
}