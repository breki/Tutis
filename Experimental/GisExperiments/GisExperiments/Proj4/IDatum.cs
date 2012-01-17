namespace GisExperiments.Proj4
{
    public interface IDatum
    {
        string DatumCode { get; }
        DatumType DatumType { get; }
        double es { get; }
        double ep2 { get; }
        IEllipsoid Ellipsoid { get; }
        string towgs84 { get; }
        string DatumName { get; }
        double[] DatumParams { get; }
        void geodetic_to_geocentric (double?[] point);
        void geocentric_to_wgs84 (double?[] point);
        void geocentric_from_wgs84 (double?[] point);
        void geocentric_to_geodetic (double?[] point);
    }
}