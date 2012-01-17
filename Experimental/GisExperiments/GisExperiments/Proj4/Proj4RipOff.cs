using System;
using Brejc.Geometry;

namespace GisExperiments.Proj4
{
    public class Proj4RipOff
    {
        public PointD2 Transform(ISrs source, ISrs dest, PointD2 point)
        {
            double?[] coords = new double?[3];
            coords[0] = point.X;
            coords[1] = point.Y;

            // DGR, 2010/11/12
            if (source.Axis != "enu")
                adjust_axis(source, false, coords);

            // Transform source points to long/lat, if they aren't already.
            //if (source.Projection.ProjectionCode == "longlat")
            //{
            //    coords[0] = GeometryUtils.Deg2Rad(coords[0].Value); // convert degrees to radians
            //    coords[1] = GeometryUtils.Deg2Rad(coords[1].Value);
            //}
            //else
            //{
                if (source.ToMeter.HasValue)
                {
                    coords[0] *= source.ToMeter.Value;
                    coords[1] *= source.ToMeter.Value;
                }

                source.Inverse(coords); // Convert Cartesian to longlat
            //}

            // Adjust for the prime meridian if necessary
            if (source.FromGreenwich.HasValue)
                coords[0] += source.FromGreenwich.Value;

            // Convert datums if needed, and if possible.
            coords = datum_transform(source.Datum, dest.Datum, coords);

            // Adjust for the prime meridian if necessary
            if (dest.FromGreenwich.HasValue)
                coords[0] -= dest.FromGreenwich.Value;

            //if (dest.Projection.ProjectionCode == "longlat")
            //{
            //    // convert radians to decimal degrees
            //    coords[0] = GeometryUtils.Rad2Deg(coords[0].Value);
            //    coords[1] = GeometryUtils.Rad2Deg(coords[1].Value);
            //}
            //else
            //{
                // else project
                dest.Forward(coords);
                if (dest.ToMeter.HasValue)
                {
                    coords[0] /= dest.ToMeter.Value;
                    coords[1] /= dest.ToMeter.Value;
                }
            //}

            // DGR, 2010/11/12
            if (dest.Axis != "enu")
                adjust_axis(dest, true, coords);

            return new PointD2(coords[0].Value, coords[1].Value);
        }

        private static void adjust_axis(ISrs crs, bool denorm, double?[] coords)
        {
            for (int i= 0; i<3; i++)
            {
                double? v = coords[i];

                switch(crs.Axis[i]) 
                {
                    case 'e':
                        coords[i]= v;
                        break;
                    case 'w':
                        coords[i]= -v;
                        break;
                    case 'n':
                        coords[i]= v;
                        break;
                    case 's':
                        coords[i]= -v;
                        break;
                    case 'u':
                        if (coords[i].HasValue)
                            coords[2] = v;
                        break;
                    case 'd':
                        if (coords[i].HasValue) 
                            coords[2] = -v;
                        break;
                    default:
                        throw new InvalidOperationException();
                        //alert("ERROR: unknow axis ("+crs.axis[i]+") - check definition of "+crs.projName);
                        //return null;
                }
            }
        }

        private static double?[] datum_transform(IDatum source, IDatum dest, double?[] coords)
        {
            // Short cut if the datums are identical.
            if (source.DatumCode == dest.DatumCode)
                return coords; 

            // Explicitly skip datum transform by setting 'datum=none' as parameter 
            // for either source or dest
            if (source.DatumType == DatumType.NoDatum
                || dest.DatumType == DatumType.NoDatum)
                return coords;

            // Do we need to go through geocentric coordinates?
            if (source.Es != dest.Es || source.Ellipsoid.SemimajorRadius != dest.Ellipsoid.SemimajorRadius
                || source.DatumType == DatumType.Param3
                || source.DatumType == DatumType.Param7
                || dest.DatumType == DatumType.Param3
                || dest.DatumType == DatumType.Param7)
            {
                // Convert to geocentric coordinates.
                source.GeodeticToGeocentric(coords);

                // CHECK_RETURN;

                // Convert between datums
                if (source.DatumType == DatumType.Param3 || source.DatumType == DatumType.Param7)
                {
                    source.GeocentricToWgs84(coords);
                    // CHECK_RETURN;
                }

                if (dest.DatumType == DatumType.Param3 || dest.DatumType == DatumType.Param7)
                {
                    dest.GeocentricFromWgs84(coords);
                    // CHECK_RETURN;
                }

                // Convert back to geodetic coordinates
                dest.GeocentricToGeodetic(coords);
                // CHECK_RETURN;
            }

            return coords;
        }
    }
}