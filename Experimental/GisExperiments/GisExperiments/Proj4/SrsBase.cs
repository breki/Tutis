namespace GisExperiments.Proj4
{
    public abstract class SrsBase : ISrs
    {
        public abstract string ProjectionCode { get;  }
        public abstract string ProjectionName { get;  }

        public double Alpha { get; set; }
        public IDatum Datum { get; set; }
        public string Axis { get; set; }
        public IEllipsoid Ellipsoid { get; set; }
        public double? FromGreenwich { get; set; }
        public double? K0 { get; set; }
        public double Lat0 { get; set; }
        public double Lat1 { get; set; }
        public double Lat2 { get; set; }
        public double LatTs { get; set; }
        public double Long0 { get; set; }
        public double LongC { get; set; }
        public string Nagrids { get; set; }
        public bool RA { get; private set; }
        public double? ToMeter { get; set; }
        public string Units { get; set; }
        public bool UtmSouth { get; set; }
        public string Title { get; set; }
        public double X0 { get; set; }
        public double Y0 { get; set; }
        public int Zone { get; set; }
        public abstract void Inverse(double?[] coords);
        public abstract void Forward(double?[] coords);

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
            if (!K0.HasValue)
                K0 = 1.0;    //default value
            //DGR 2010-11-12: axis
            if (Axis == null)
                Axis = "enu";

            //datum = new Proj4js.datum (this);
        }
    }
}