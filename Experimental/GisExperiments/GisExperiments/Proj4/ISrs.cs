namespace GisExperiments.Proj4
{
    public interface ISrs : IProjection
    {
        double Alpha { get; set; }
        string Axis { get; set; }
        IDatum Datum { get; set; }
        IEllipsoid Ellipsoid { get; set; }
        double? FromGreenwich { get; set; }
        double? K0 { get; set; }
        double Lat0 { get; set; }
        double Lat1 { get; set; }
        double Lat2 { get; set; }
        double LatTs { get; set; }
        double Long0 { get; set; }
        double LongC { get; set; }
        string Nagrids { get; set; }
        bool RA { get; }
        string Title { get; set; }
        double? ToMeter { get; set; }
        string Units { get; set; }
        bool UtmSouth { get; set; }
        double X0 { get; set; }
        double Y0 { get; set; }
        int Zone { get; set; }

        void Initialize();
    }
}