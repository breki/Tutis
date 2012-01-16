using System;
using Brejc.Geometry;

namespace GisExperiments.Proj4
{
    public enum DatumType
    {
        NoDatum,
        Param3,
        Param7
    }

    public interface IDatum
    {
        bool compare_datums(IDatum dest);
        DatumType datum_type { get; }
        double es { get; }
        double a { get; }
        void geodetic_to_geocentric(double?[] point);
        void geocentric_to_wgs84(double?[] point);
        void geocentric_from_wgs84(double?[] point);
        void geocentric_to_geodetic(double?[] point);
    }

    public interface ISrs
    {
        string axis { get; }
        string projName { get; }
        double? to_meter { get; }
        double? from_greenwich { get; }
        IDatum datum { get; }

        // Convert Cartesian to longlat
        void inverse(PointD2 point);

        void forward(PointD2 point);
    }

    public class Proj4RipOff
    {
        public PointD2 Transform(ISrs source, ISrs dest, PointD2 point)
        {
            double?[] coords = new double?[3];
            coords[0] = point.X;
            coords[1] = point.Y;

            // DGR, 2010/11/12
            if (source.axis != "enu")
                adjust_axis(source, false, coords);

            // Transform source points to long/lat, if they aren't already.
            if (source.projName == "longlat")
            {
                coords[0] = GeometryUtils.Deg2Rad(coords[0].Value); // convert degrees to radians
                coords[1] = GeometryUtils.Deg2Rad(coords[1].Value);
            }
            else
            {
                if (source.to_meter.HasValue)
                {
                    coords[0] *= source.to_meter.Value;
                    coords[1] *= source.to_meter.Value;
                }

                source.inverse(point); // Convert Cartesian to longlat
            }

            // Adjust for the prime meridian if necessary
            if (source.from_greenwich.HasValue)
                coords[0] += source.from_greenwich.Value;

            // Convert datums if needed, and if possible.
            coords = datum_transform(source.datum, dest.datum, coords);

            // Adjust for the prime meridian if necessary
            if (dest.from_greenwich.HasValue)
                coords[0] -= dest.from_greenwich.Value;

            if (dest.projName == "longlat")
            {
                // convert radians to decimal degrees
                coords[0] = GeometryUtils.Rad2Deg(coords[0].Value);
                coords[1] = GeometryUtils.Rad2Deg(coords[1].Value);
            }
            else
            {
                // else project
                dest.forward(point);
                if (dest.to_meter.HasValue)
                {
                    coords[0] /= dest.to_meter.Value;
                    coords[1] /= dest.to_meter.Value;
                }
            }

            // DGR, 2010/11/12
            if (dest.axis != "enu")
                adjust_axis(dest, true, coords);

            return new PointD2(coords[0].Value, coords[1].Value);
        }

        private void adjust_axis(ISrs crs, bool denorm, double?[] coords)
        {
            for (int i= 0; i<3; i++)
            {
                double? v = coords[i];

                switch(crs.axis[i]) 
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

        private double?[] datum_transform(IDatum source, IDatum dest, double?[] coords)
        {
            // Short cut if the datums are identical.
            if (source.compare_datums(dest))
            {
                return coords; // in this case, zero is sucess,
                // whereas cs_compare_datums returns 1 to indicate TRUE
                // confusing, should fix this
            }

            // Explicitly skip datum transform by setting 'datum=none' as parameter 
            // for either source or dest
            if (source.datum_type == DatumType.NoDatum
                || dest.datum_type == DatumType.NoDatum)
                return coords;

            // Do we need to go through geocentric coordinates?
            if (source.es != dest.es || source.a != dest.a
                || source.datum_type == DatumType.Param3
                || source.datum_type == DatumType.Param7
                || dest.datum_type == DatumType.Param3
                || dest.datum_type == DatumType.Param7)
            {
                // Convert to geocentric coordinates.
                source.geodetic_to_geocentric(coords);

                // CHECK_RETURN;

                // Convert between datums
                if (source.datum_type == DatumType.Param3 || source.datum_type == DatumType.Param7)
                {
                    source.geocentric_to_wgs84(coords);
                    // CHECK_RETURN;
                }

                if (dest.datum_type == DatumType.Param3 || dest.datum_type == DatumType.Param7)
                {
                    dest.geocentric_from_wgs84(coords);
                    // CHECK_RETURN;
                }

                // Convert back to geodetic coordinates
                dest.geocentric_to_geodetic(coords);
                // CHECK_RETURN;
            }

            return coords;
        }
    }
}