namespace GisExperiments.Proj4
{
    public interface ISrs : IProjection
    {
        //double? a { get; }
        double alpha { get; set; }
        //double? b { get; }
        string axis { get; set; }
        IDatum Datum { get; set; }
        //string datumCode { get; }
        //double[] datum_params { get; }
        //string ellps { get; }
        IEllipsoid Ellipsoid { get; set; }
        //double es { get; }
        //double ep2 { get; }
        double? from_greenwich { get; set; }
        double? k0 { get; set; }
        double lat0 { get; set; }
        double lat1 { get; set; }
        double lat2 { get; set; }
        double lat_ts { get; set; }
        double long0 { get; set; }
        double longc { get; set; }
        string nagrids { get; set; }
        //double? rf { get; }
        bool R_A { get; }
        string title { get; set; }
        double? to_meter { get; set; }
        string units { get; set; }
        bool utmSouth { get; set; }
        double x0 { get; set; }
        double y0 { get; set; }
        int zone { get; set; }

        void Initialize();
    }
}