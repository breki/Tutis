using Brejc.Geometry;

namespace GisExperiments.Proj4
{
    public class LongLatProjection : IProjection
    {
        public string ProjectionCode
        {
            get { return "longlat"; }
        }

        public string ProjectionName
        {
            get { return "LongLat"; }
        }

        public void inverse (double?[] coords)
        {
            coords[0] = GeometryUtils.Deg2Rad (coords[0].Value);
            coords[1] = GeometryUtils.Deg2Rad (coords[1].Value);
        }

        public void forward (double?[] coords)
        {
            coords[0] = GeometryUtils.Rad2Deg(coords[0].Value);
            coords[1] = GeometryUtils.Rad2Deg (coords[0].Value);
        }
    }
}