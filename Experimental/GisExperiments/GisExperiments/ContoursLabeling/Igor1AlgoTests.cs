using System;
using Brejc.Common.FileSystem;
using Brejc.Geometry.ExternalFormats.Dbase;
using Brejc.Geometry.ExternalFormats.Shapefiles;
using MbUnit.Framework;

namespace GisExperiments.ContoursLabeling
{
    public class Igor1AlgoTests
    {
        [Test]
        public void Test()
        {
            ContoursLabels labels = algo.LabelContours(contours, parameters);
        }

        [SetUp]
        public void Setup()
        {
            algo = new Igor1ContoursLabelingAlgorithm();
            parameters = new ContoursLabelingParameters();
            parameters.ElevationInterval = 250;

            contours = FetchContours();
        }

        private IContoursSet FetchContours()
        {
            WindowsFileSystem fileSystem = new WindowsFileSystem();
            ShapefileReader reader = new ShapefileReader(fileSystem, new DbaseFileReader(fileSystem));
            ShapefileFile file = reader.Read(@"D:\MyStuff\Dropbox\Work\Maperitive\Projects\SloveniaPy\artifacts\contours25_50.shp");

            ContoursSet set = new ContoursSet();

            foreach (IShape shape in file.Shapes)
            {
                int a = 0;
                a++;
            }

            throw new NotImplementedException();
        }

        private Igor1ContoursLabelingAlgorithm algo;
        private IContoursSet contours;
        private ContoursLabelingParameters parameters;
    }
}