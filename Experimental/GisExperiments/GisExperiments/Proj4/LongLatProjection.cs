using Brejc.Geometry;

namespace GisExperiments.Proj4
{
    public class LongLatProjection : SrsBase
    {
        public override string ProjectionCode
        {
            get { return "longlat"; }
        }

        public override string ProjectionName
        {
            get { return "LongLat"; }
        }

        public override void Inverse (double?[] coords)
        {
            coords[0] = GeometryUtils.Deg2Rad (coords[0].Value);
            coords[1] = GeometryUtils.Deg2Rad (coords[1].Value);
        }

        public override void Forward (double?[] coords)
        {
            coords[0] = GeometryUtils.Rad2Deg(coords[0].Value);
            coords[1] = GeometryUtils.Rad2Deg (coords[0].Value);
        }
    }
}