using Brejc.Common.FileSystem;
using Brejc.OsmLibrary;
using SpatialitePlaying.CustomPbf;
using NUnit.Framework;

namespace SpatialitePlaying
{
    public class Tests
    {
        [Test]
        public void AnalyzeOsmFile()
        {
            OsmFileAnalyzer analyzer = new OsmFileAnalyzer();

            WindowsFileSystem fileSystem = new WindowsFileSystem ();
            using (IOsmReader osmReader = new OsmPbfReader2())
            {
                osmReader.Settings.IgnoreCreatedByTags = true;
                osmReader.Settings.LoadExtendedData = false;

                //osmReader.Read (@"D:\brisi\isle-of-man-latest.osm.pbf", fileSystem, osmDb);
                osmReader.Read (@"D:\brisi\slovenia-latest.osm.pbf", fileSystem, analyzer);
                //osmReader.Read (@"D:\brisi\austria-latest.osm.pbf", fileSystem, analyzer);
                //osmReader.Read (@"D:\brisi\germany-latest.osm.pbf", fileSystem, analyzer);
            }

            Assert.IsTrue(analyzer.AreEntityTypesMonotone);
            Assert.IsTrue(analyzer.AreNodeIdsMonotone);
            Assert.IsTrue(analyzer.AreRelationIdsMonotone);
            Assert.IsTrue(analyzer.AreWaysIdsMonotone);
        }
    }
}
