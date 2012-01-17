namespace GisExperiments.Proj4
{
    public interface IDatum
    {
        string DatumCode { get; }
        DatumType DatumType { get; }
        string DatumName { get; }
        double Es { get; }
        double Ep2 { get; }
        IEllipsoid Ellipsoid { get; }
        double[] DatumParams { get; }

        void GeodeticToGeocentric (double?[] point);
        void GeocentricToWgs84 (double?[] point);
        void GeocentricFromWgs84 (double?[] point);
        void GeocentricToGeodetic (double?[] point);
    }
}