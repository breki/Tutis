using System.Collections.Generic;
using Brejc.Common.FileSystem;
using Brejc.Geometry;
using Brejc.Geometry.ExternalFormats.Dbase;
using Brejc.Geometry.ExternalFormats.Shapefiles;
using MbUnit.Framework;
using Rhino.Mocks;

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
            IContoursLabelMeasurer labelMeasurer = MockRepository.GenerateStub<IContoursLabelMeasurer>();
            labelMeasurer.Stub(x => x.CalculateLabelWidth(0, null))
                .IgnoreArguments().Return(100);

            algo = new Igor1ContoursLabelingAlgorithm();
            parameters = new ContoursLabelingParameters();
            parameters.ContoursLabelMeasurer = labelMeasurer;
            parameters.ElevationInterval = 250;

            contours = FetchContours();
        }

        private IContoursSet FetchContours()
        {
            WindowsFileSystem fileSystem = new WindowsFileSystem();
            ShapefileReader reader = new ShapefileReader(fileSystem, new DbaseFileReader(fileSystem));
            ShapefileFile file = reader.Read(
                //@"D:\MyStuff\Dropbox\Work\Maperitive\Projects\SloveniaPy\artifacts\contours25_50.shp");
                @"D:\MyStuff\My Dropbox\Work\Maperitive\Projects\SloveniaPy\artifacts\contours25_50.shp");

            ContoursSet set = new ContoursSet();

            for (int i = 0; i < file.ShapesCount; i++)
            {
                IShape shape = file.Shapes[i];
                double elevation = file.Attributes.GetValue<double>(i, "ele");
                ContoursForElevation contoursForElevation = new ContoursForElevation(elevation);
                set.AddElevation(contoursForElevation);

                PolyLine polyLine = (PolyLine)shape;
                for (int j = 0; j < polyLine.Parts.Length; j++)
                {
                    int startIndex = polyLine.Parts[j];
                    int endIndex = j < polyLine.Parts.Length - 1 ? polyLine.Parts[j + 1] : polyLine.Points.Length;

                    List<IPointF2> contourPoints = new List<IPointF2>();
                    for (int k = 0; k < endIndex - startIndex; k++)
                    {
                        IPointD2 point = polyLine.Points[startIndex + k];
                        contourPoints.Add(new PointF2((float)point.X, (float)point.Y));
                    }

                    ContourLine contourLine = new ContourLine(contourPoints);
                    contoursForElevation.AddContourLine(contourLine);
                }
            }

            return set;
        }

        private Igor1ContoursLabelingAlgorithm algo;
        private IContoursSet contours;
        private ContoursLabelingParameters parameters;
    }
}