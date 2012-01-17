using System;
using System.Globalization;
using Brejc.Geometry;

namespace GisExperiments.Proj4
{
    public class GenericSrs : ISrs
    {
        //private string datumName;
        //private bool sphere;
        //private double a2;
        //private double b2;
        //public double es { get; private set; }
        //private double e;
        //public double ep2 { get; private set; }
        public string axis { get; private set; }
        //public string projName { get; private set; }
        public double? to_meter { get; private set; }
        public double? from_greenwich { get; private set; }
        public IDatum Datum { get; private set; }
        public string title { get; private set; }
        //public double? a { get; private set; }
        public double alpha { get; private set; }
        //public double? b { get; private set; }
        //public string datumCode { get; private set; }
        //public double[] datum_params { get; private set; }
        public IEllipsoid Ellipsoid { get; private set; }
        //public string ellps { get; private set; }
        public double? k0 { get; private set; }
        public double lat0 { get; private set; }
        public double lat1 { get; private set; }
        public double lat2 { get; private set; }
        public double lat_ts { get; private set; }
        public double long0 { get; private set; }
        public double longc { get; private set; }
        public string nagrids { get; private set; }
        //public double? rf { get; private set; }
        public bool R_A { get; private set; }
        public string units { get; private set; }
        public bool utmSouth { get; private set; }
        public double x0 { get; private set; }
        public double y0 { get; private set; }
        public int zone { get; private set; }
        public IProjection Projection { get; private set; }

        public void inverse (PointD2 point)
        {
            throw new System.NotImplementedException ();
        }

        public void forward (PointD2 point)
        {
        }

        public static ISrs ParseProj4 (string proj4String)
        {
            GenericSrs srs = new GenericSrs ();
            string[] paramArray = proj4String.Split ('+');

            for (int prop = 0; prop < paramArray.Length; prop++)
            {
                string[] property = paramArray[prop].Split ('=');
                string paramName = property[0].ToLower ().Trim ();
                string paramVal = null;

                if (property.Length > 1)
                    paramVal = property[1].Trim ();

                double paramValDouble;
                bool doubleParsed = double.TryParse (paramVal, NumberStyles.Number, CultureInfo.InvariantCulture, out paramValDouble);

                switch (paramName)
                {
                    case "":
                        break;   // throw away nameless parameter
                    case "title":
                        srs.title = paramVal;
                        break;
                    case "proj":
                        srs.Projection = Projections.GetProjection(paramVal);
                        break;
                    case "units":
                        srs.units = paramVal;
                        break;
                    case "datum":
                        srs.Datum = Datums.GetDatum(paramVal);
                        break;
                    case "nadgrids":
                        srs.nagrids = paramVal;
                        break;
                    case "ellps":
                        srs.Ellipsoid = Ellipsoids.GetEllipsoid(paramVal);
                        break;
                    case "a":
                        throw new NotSupportedException();
                        //srs.a = paramValDouble;
                        //break;  // semi-major radius
                    case "b":
                        throw new NotSupportedException ();
                        //srs.b = paramValDouble;
                        //break;  // semi-minor radius
                    // DGR 2007-11-20
                    case "rf":
                        throw new NotSupportedException();
                        //srs.rf = paramValDouble;
                        //break; // inverse flattening rf= a/(a-b)
                    case "lat_0":
                        srs.lat0 = GeometryUtils.Deg2Rad (paramValDouble);
                        break;        // phi0, central latitude
                    case "lat_1":
                        srs.lat1 = GeometryUtils.Deg2Rad (paramValDouble);
                        break;        //standard parallel 1
                    case "lat_2":
                        srs.lat2 = GeometryUtils.Deg2Rad (paramValDouble);
                        break;        //standard parallel 2
                    case "lat_ts":
                        srs.lat_ts = GeometryUtils.Deg2Rad (paramValDouble);
                        break;      // used in merc and eqc
                    case "lon_0":
                        srs.long0 = GeometryUtils.Deg2Rad (paramValDouble);
                        break;       // lam0, central longitude
                    case "alpha":
                        srs.alpha = GeometryUtils.Deg2Rad (paramValDouble);
                        break;  //for somerc projection
                    case "lonc":
                        srs.longc = GeometryUtils.Deg2Rad (paramValDouble);
                        break;       //for somerc projection
                    case "x_0":
                        srs.x0 = paramValDouble;
                        break;  // false easting
                    case "y_0":
                        srs.y0 = paramValDouble;
                        break;  // false northing
                    case "k_0":
                        srs.k0 = paramValDouble;
                        break;  // projection scale factor
                    case "k":
                        srs.k0 = paramValDouble;
                        break;  // both forms returned
                    case "r_a":
                        throw new NotImplementedException();
                        //srs.R_A = true;
                        //break;                 // sphere--area of ellipsoid
                    case "zone":
                        srs.zone = (int)paramValDouble;
                        break;  // UTM Zone
                    case "south":
                        srs.utmSouth = true;
                        break;  // UTM north/south
                    case "towgs84":
                        throw new NotImplementedException();
                        //srs.datum_params = Proj4.Datum.ParseDatumParams(paramVal);
                        //break;
                    case "to_meter":
                        srs.to_meter = paramValDouble;
                        break; // cartesian scaling
                    case "from_greenwich":
                        srs.from_greenwich = GeometryUtils.Deg2Rad (paramValDouble);
                        break;
                    // DGR 2008-07-09 : if pm is not a well-known prime meridian take
                    // the value instead of 0.0, then convert to radians
                    case "pm":
                        throw new NotImplementedException ();
                    //srs.from_greenwich = Proj4js.PrimeMeridian[paramVal] ?
                    //                                                         Proj4js.PrimeMeridian[paramVal] : parseFloat(paramVal);
                    //srs.from_greenwich = GeometryUtils.Deg2Rad(srs.from_greenwich.Value); 
                    //break;
                    // DGR 2010-11-12: axis
                    case "axis":
                        string legalAxis = "ewnsud";
                        if (paramVal.Length == 3 &&
                            legalAxis.IndexOf (paramVal.Substring (0, 1)) != -1 &&
                            legalAxis.IndexOf (paramVal.Substring (1, 1)) != -1 &&
                            legalAxis.IndexOf (paramVal.Substring (2, 1)) != -1)
                            srs.axis = paramVal;
                        //FIXME: be silent ?
                        break;
                    case "no_defs":
                        break;
                    default:
                        throw new NotImplementedException ();
                    //alert("Unrecognized parameter: " + paramName);
                } // switch()
            } // for paramArray

            srs.DeriveConstants ();

            return srs;
        }

        private void DeriveConstants ()
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