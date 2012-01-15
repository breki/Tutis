using DotSpatial.Projections;
using Gallio.Framework;
using MbUnit.Framework;

namespace GisExperiments
{
    public class DotSpatialProjectionTests
    {
        [Test]
        public void Test()
        {
            ProjectionInfo p = new ProjectionInfo();

            ProjectionInfo pStart = KnownCoordinateSystems.Geographic.World.WGS1984;
            //ProjectionInfo pEnd = new ProjectionInfo ("+proj=tmerc +lat_0=0 +lon_0=15 +k=0.9999 +x_0=500000 +y_0=0 +ellps=bessel +towgs84=577.326,90.129,463.919,5.137,1.474,5.297,2.4232 +units=m +no_defs");
            ProjectionInfo pEnd = new ProjectionInfo();

            //pEnd = new ProjectionInfo ("+proj=tmerc +lat_0=0 +lon_0=15 +k=0.9999 +x_0=500000 +y_0=-5000000 +ellps=bessel +towgs84=577.326,90.129,463.919,5.137,1.474,5.297,2.4232 +units=m +no_defs");
            pEnd.ReadEsriString(@"PROJCS[""MGI / Slovene National Grid"",
    GEOGCS[""MGI"",
        DATUM[""Militar_Geographische_Institute"",
            SPHEROID[""Bessel 1841"",6377397.155,299.1528128,
                AUTHORITY[""EPSG"",""7004""]],
            TOWGS84[577.326,90.129,463.919,5.137,1.474,5.297,2.4232],
            AUTHORITY[""EPSG"",""6312""]],
        PRIMEM[""Greenwich"",0,
            AUTHORITY[""EPSG"",""8901""]],
        UNIT[""degree"",0.0174532925199433,
            AUTHORITY[""EPSG"",""9108""]],
        AUTHORITY[""EPSG"",""4312""]],
    UNIT[""metre"",1,
        AUTHORITY[""EPSG"",""9001""]],
    PROJECTION[""Transverse_Mercator""],
    PARAMETER[""latitude_of_origin"",0],
    PARAMETER[""central_meridian"",15],
    PARAMETER[""scale_factor"",0.9999],
    PARAMETER[""false_easting"",500000],
    PARAMETER[""false_northing"",-5000000],
    AUTHORITY[""EPSG"",""3787""],
    AXIS[""Y"",EAST],
    AXIS[""X"",NORTH]]");

            //TestLog.WriteLine(pEnd.ToString());
            //TestLog.WriteLine(pEnd.ToEsriString());

            //Declares the point to be project, starts out as 0,0
            double[] xy = new double[2];
            double[] z = new double[1];

            xy[0] = 15.608;
            xy[1] = 46.279;
            //calls the reproject function and reprojects the points
            Reproject.ReprojectPoints (xy, z, pStart, pEnd, 0, 1);

            TestLog.WriteLine("{0}, {1}", xy[0], xy[1]);
        }

        [Test]
        public void Wgs84ToOsgb()
        {
            ProjectionInfo p = new ProjectionInfo();

            ProjectionInfo pStart = new ProjectionInfo("+proj=longlat +ellps=WGS84 +datum=WGS84 +no_defs");
            ProjectionInfo pEnd = new ProjectionInfo("+proj=tmerc +lat_0=49 +lon_0=-2 +k=0.9996012717 +x_0=400000 +y_0=-100000 +ellps=airy +towgs84=446.448,-125.157,542.060,0.1502,0.2470,0.8421,-20.4894 +units=m +no_defs <>");
              //ProjectionInfo pEnd = new ProjectionInfo("+proj=tmerc +lat_0=49 +lon_0=-2 +k=0.9996012717 +x_0=400000 +y_0=-100000 +ellps=airy +datum=OSGB36 +units=m +no_defs");

            //ProjectionInfo pEnd = KnownCoordinateSystems.Geographic.Europe.OSGB1936;
//            ProjectionInfo pEnd = new ProjectionInfo();

//            pEnd.ReadEsriString(@"PROJCS[""OSGB 1936 / British National Grid"",
//    GEOGCS[""OSGB 1936"",
//        DATUM[""OSGB_1936"",
//            SPHEROID[""Airy 1830"",6377563.396,299.3249646,
//                AUTHORITY[""EPSG"",""7001""]],
//            AUTHORITY[""EPSG"",""6277""]],
//        PRIMEM[""Greenwich"",0,
//            AUTHORITY[""EPSG"",""8901""]],
//        UNIT[""degree"",0.01745329251994328,
//            AUTHORITY[""EPSG"",""9122""]],
//        AUTHORITY[""EPSG"",""4277""]],
//    UNIT[""metre"",1,
//        AUTHORITY[""EPSG"",""9001""]],
//    PROJECTION[""Transverse_Mercator""],
//    PARAMETER[""latitude_of_origin"",49],
//    PARAMETER[""central_meridian"",-2],
//    PARAMETER[""scale_factor"",0.9996012717],
//    PARAMETER[""false_easting"",400000],
//    PARAMETER[""false_northing"",-100000],
//    AUTHORITY[""EPSG"",""27700""],
//    AXIS[""Easting"",EAST],
//    AXIS[""Northing"",NORTH]]");

            double[] xy = new double[2];
            double[] z = new double[1];

            xy[0] = -0.10322;
            xy[1] = 51.52237;
            //calls the reproject function and reprojects the points
            Reproject.ReprojectPoints(xy, z, pStart, pEnd, 0, 1);

            TestLog.WriteLine("{0}, {1}", xy[0], xy[1]);

            // http://www.fieldenmaps.info/cconv/cconv_gb.html
            Assert.AreEqual(531689.532, xy[0]);
            Assert.AreEqual(182088.483, xy[1]);
        }
    }
}