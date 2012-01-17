using Brejc.Geometry;
using MbUnit.Framework;

namespace GisExperiments.Proj4
{
    public class Proj4RipOffTests
    {
        [Test]
        public void Test()
        {
            Proj4RipOff proj4 = new Proj4RipOff();

            ISrs source = GenericSrs.ParseProj4 ("+proj=longlat +ellps=WGS84 +datum=WGS84 +no_defs");
            ISrs dest = GenericSrs.ParseProj4 ("+proj=tmerc +lat_0=49 +lon_0=-2 +k=0.9996012717 +x_0=400000 +y_0=-100000 +ellps=airy +datum=OSGB36 +units=m +no_defs");

            PointD2 transformedPoint = proj4.Transform(source, dest, new PointD2(-0.10322, 51.52237));
            Assert.AreEqual (531689.532, transformedPoint.X);
            Assert.AreEqual (182088.483, transformedPoint.Y);
        }     
    }
}