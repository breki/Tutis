using Brejc.Geometry;

namespace GisExperiments.Proj4
{
    public interface ISrs
    {
        //double? a { get; }
        double alpha { get; }
        //double? b { get; }
        string axis { get; }
        IDatum Datum { get; }
        //string datumCode { get; }
        //double[] datum_params { get; }
        //string ellps { get; }
        IEllipsoid Ellipsoid { get; }
        //double es { get; }
        //double ep2 { get; }
        double? from_greenwich { get; }
        double? k0 { get; }
        double lat0 { get; }
        double lat1 { get; }
        double lat2 { get; }
        double lat_ts { get; }
        double long0 { get; }
        double longc { get; }
        string nagrids { get; }
        IProjection Projection { get; }
        //double? rf { get; }
        bool R_A { get; }
        string title { get; }
        double? to_meter { get; }
        string units { get; }
        bool utmSouth { get; }
        double x0 { get; }
        double y0 { get; }
        int zone { get; }
    }
}