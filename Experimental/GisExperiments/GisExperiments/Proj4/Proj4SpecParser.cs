using System;
using System.Collections.Generic;
using System.Globalization;
using Brejc.Geometry;

namespace GisExperiments.Proj4
{
    public class Proj4SpecParser
    {
        public ISrs ParseSrsSpecification (string spec)
        {
            List<SrsParameter> parameters = ExtractParameters (spec);

            ISrs srs = FindProjection (parameters);
            FillSrsParameters (srs, parameters);

            srs.Initialize();

            //for (int prop = 0; prop < paramArray.Length; prop++)
            //{
            //    string[] property = paramArray[prop].Split ('=');
            //    string paramName = property[0].ToLower ().Trim ();
            //    string paramVal = null;

            //    if (property.Length > 1)
            //        paramVal = property[1].Trim ();

            //    double paramValDouble;
            //    bool doubleParsed = double.TryParse (paramVal, NumberStyles.Number, CultureInfo.InvariantCulture, out paramValDouble);

            //    switch (paramName)
            //    {
            //        case "":
            //            break;   // throw away nameless parameter
            //        case "title":
            //            srs.title = paramVal;
            //            break;
            //        case "proj":
            //            srs.Projection = Projections.GetProjection (paramVal);
            //            break;
            //        case "units":
            //            srs.units = paramVal;
            //            break;
            //        case "datum":
            //            srs.Datum = Datums.GetDatum (paramVal);
            //            break;
            //        case "nadgrids":
            //            srs.nagrids = paramVal;
            //            break;
            //        case "ellps":
            //            srs.Ellipsoid = Ellipsoids.GetEllipsoid (paramVal);
            //            break;
            //        case "a":
            //            throw new NotSupportedException ();
            //            //srs.a = paramValDouble;
            //            //break;  // semi-major radius
            //        case "b":
            //            throw new NotSupportedException ();
            //            //srs.b = paramValDouble;
            //            //break;  // semi-minor radius
            //            // DGR 2007-11-20
            //        case "rf":
            //            throw new NotSupportedException ();
            //            //srs.rf = paramValDouble;
            //            //break; // inverse flattening rf= a/(a-b)
            //        case "lat_0":
            //            srs.lat0 = GeometryUtils.Deg2Rad (paramValDouble);
            //            break;        // phi0, central latitude
            //        case "lat_1":
            //            srs.lat1 = GeometryUtils.Deg2Rad (paramValDouble);
            //            break;        //standard parallel 1
            //        case "lat_2":
            //            srs.lat2 = GeometryUtils.Deg2Rad (paramValDouble);
            //            break;        //standard parallel 2
            //        case "lat_ts":
            //            srs.lat_ts = GeometryUtils.Deg2Rad (paramValDouble);
            //            break;      // used in merc and eqc
            //        case "lon_0":
            //            srs.long0 = GeometryUtils.Deg2Rad (paramValDouble);
            //            break;       // lam0, central longitude
            //        case "alpha":
            //            srs.alpha = GeometryUtils.Deg2Rad (paramValDouble);
            //            break;  //for somerc projection
            //        case "lonc":
            //            srs.longc = GeometryUtils.Deg2Rad (paramValDouble);
            //            break;       //for somerc projection
            //        case "x_0":
            //            srs.x0 = paramValDouble;
            //            break;  // false easting
            //        case "y_0":
            //            srs.y0 = paramValDouble;
            //            break;  // false northing
            //        case "k_0":
            //            srs.k0 = paramValDouble;
            //            break;  // projection scale factor
            //        case "k":
            //            srs.k0 = paramValDouble;
            //            break;  // both forms returned
            //        case "r_a":
            //            throw new NotImplementedException ();
            //            //srs.R_A = true;
            //            //break;                 // sphere--area of ellipsoid
            //        case "zone":
            //            srs.zone = (int)paramValDouble;
            //            break;  // UTM Zone
            //        case "south":
            //            srs.utmSouth = true;
            //            break;  // UTM north/south
            //        case "towgs84":
            //            throw new NotImplementedException ();
            //            //srs.datum_params = Proj4.Datum.ParseDatumParams(paramVal);
            //            //break;
            //        case "to_meter":
            //            srs.to_meter = paramValDouble;
            //            break; // cartesian scaling
            //        case "from_greenwich":
            //            srs.from_greenwich = GeometryUtils.Deg2Rad (paramValDouble);
            //            break;
            //            // DGR 2008-07-09 : if pm is not a well-known prime meridian take
            //            // the value instead of 0.0, then convert to radians
            //        case "pm":
            //            throw new NotImplementedException ();
            //            //srs.from_greenwich = Proj4js.PrimeMeridian[paramVal] ?
            //            //                                                         Proj4js.PrimeMeridian[paramVal] : parseFloat(paramVal);
            //            //srs.from_greenwich = GeometryUtils.Deg2Rad(srs.from_greenwich.Value); 
            //            //break;
            //            // DGR 2010-11-12: axis
            //        case "axis":
            //            string legalAxis = "ewnsud";
            //            if (paramVal.Length == 3 &&
            //                legalAxis.IndexOf (paramVal.Substring (0, 1)) != -1 &&
            //                legalAxis.IndexOf (paramVal.Substring (1, 1)) != -1 &&
            //                legalAxis.IndexOf (paramVal.Substring (2, 1)) != -1)
            //                srs.axis = paramVal;
            //            //FIXME: be silent ?
            //            break;
            //        case "no_defs":
            //            break;
            //        default:
            //            throw new NotImplementedException ();
            //            //alert("Unrecognized parameter: " + paramName);
            //    } // switch()
            //} // for paramArray

            //srs.DeriveConstants ();

            return srs;
        }

        private static List<SrsParameter> ExtractParameters (string spec)
        {
            string[] paramArray = spec.Split ('+');
            List<SrsParameter> parameters = new List<SrsParameter> ();

            for (int i = 0; i < paramArray.Length; i++)
            {
                string[] property = paramArray[i].Split ('=');
                string paramName = property[0].ToLower ().Trim ();
                string paramVal = null;

                if (property.Length > 1)
                    paramVal = property[1].Trim ();

                SrsParameter parameter = new SrsParameter (paramName);
                parameter.StringValue = paramVal;

                double paramValDouble;
                bool doubleParsed = double.TryParse (paramVal, NumberStyles.Number, CultureInfo.InvariantCulture, out paramValDouble);

                if (doubleParsed)
                    parameter.NumericValue = paramValDouble;

                parameters.Add (parameter);
            }

            return parameters;
        }

        private ISrs FindProjection (List<SrsParameter> parameters)
        {
            SrsParameter srsParameter = parameters.Find (x => x.Name == "proj");
            return (ISrs)Projections.GetProjection (srsParameter.StringValue);
        }

        private void FillSrsParameters (ISrs srs, List<SrsParameter> parameters)
        {
            foreach (SrsParameter parameter in parameters)
            {
                switch (parameter.Name)
                {
                    case "":
                        break;   // throw away nameless parameter
                    case "title":
                        srs.title = parameter.StringValue;
                        break;
                    case "proj":
                        break;
                    case "units":
                        srs.units = parameter.StringValue;
                        break;
                    case "datum":
                        srs.Datum = Datums.GetDatum (parameter.StringValue);
                        break;
                    case "nadgrids":
                        srs.nagrids = parameter.StringValue;
                        break;
                    case "ellps":
                        srs.Ellipsoid = Ellipsoids.GetEllipsoid (parameter.StringValue);
                        break;
                    case "a":
                        throw new NotSupportedException ();
                    //srs.a = paramValDouble;
                    //break;  // semi-major radius
                    case "b":
                        throw new NotSupportedException ();
                    //srs.b = paramValDouble;
                    //break;  // semi-minor radius
                    // DGR 2007-11-20
                    case "rf":
                        throw new NotSupportedException ();
                    //srs.rf = paramValDouble;
                    //break; // inverse flattening rf= a/(a-b)
                    case "lat_0":
                        srs.lat0 = GeometryUtils.Deg2Rad (parameter.NumericValue.Value);
                        break;        // phi0, central latitude
                    case "lat_1":
                        srs.lat1 = GeometryUtils.Deg2Rad (parameter.NumericValue.Value);
                        break;        //standard parallel 1
                    case "lat_2":
                        srs.lat2 = GeometryUtils.Deg2Rad (parameter.NumericValue.Value);
                        break;        //standard parallel 2
                    case "lat_ts":
                        srs.lat_ts = GeometryUtils.Deg2Rad (parameter.NumericValue.Value);
                        break;      // used in merc and eqc
                    case "lon_0":
                        srs.long0 = GeometryUtils.Deg2Rad (parameter.NumericValue.Value);
                        break;       // lam0, central longitude
                    case "alpha":
                        srs.alpha = GeometryUtils.Deg2Rad (parameter.NumericValue.Value);
                        break;  //for somerc projection
                    case "lonc":
                        srs.longc = GeometryUtils.Deg2Rad (parameter.NumericValue.Value);
                        break;       //for somerc projection
                    case "x_0":
                        srs.x0 = parameter.NumericValue.Value;
                        break;  // false easting
                    case "y_0":
                        srs.y0 = parameter.NumericValue.Value;
                        break;  // false northing
                    case "k_0":
                        srs.k0 = parameter.NumericValue.Value;
                        break;  // projection scale factor
                    case "k":
                        srs.k0 = parameter.NumericValue.Value;
                        break;  // both forms returned
                    case "r_a":
                        throw new NotImplementedException ();
                    //srs.R_A = true;
                    //break;                 // sphere--area of ellipsoid
                    case "zone":
                        srs.zone = (int)Math.Round(parameter.NumericValue.Value);
                        break;  // UTM Zone
                    case "south":
                        srs.utmSouth = true;
                        break;  // UTM north/south
                    case "towgs84":
                        throw new NotImplementedException ();
                    //srs.datum_params = Proj4.Datum.ParseDatumParams(paramVal);
                    //break;
                    case "to_meter":
                        srs.to_meter = parameter.NumericValue.Value;
                        break; // cartesian scaling
                    case "from_greenwich":
                        srs.from_greenwich = GeometryUtils.Deg2Rad (parameter.NumericValue.Value);
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
                        if (parameter.StringValue.Length == 3 &&
                            legalAxis.IndexOf (parameter.StringValue.Substring (0, 1)) != -1 &&
                            legalAxis.IndexOf (parameter.StringValue.Substring (1, 1)) != -1 &&
                            legalAxis.IndexOf (parameter.StringValue.Substring (2, 1)) != -1)
                            srs.axis = parameter.StringValue;
                        //FIXME: be silent ?
                        break;
                    case "no_defs":
                        break;
                    default:
                        throw new NotImplementedException ();
                    //alert("Unrecognized parameter: " + paramName);
                } // switch()
            }
        }
    }
}