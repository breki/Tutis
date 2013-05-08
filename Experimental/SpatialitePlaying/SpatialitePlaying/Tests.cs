using Brejc.Common.FileSystem;
using Brejc.OsmLibrary.Pbf;
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
            using (OsmPbfReader osmReader = new OsmPbfReader ())
            {
                osmReader.Settings.IgnoreCreatedByTags = true;
                osmReader.Settings.LoadExtendedData = false;

                //osmReader.Read (@"D:\brisi\isle-of-man-latest.osm.pbf", fileSystem, osmDb);
                //osmReader.Read (@"D:\brisi\slovenia-latest.osm.pbf", fileSystem, analyzer);
                osmReader.Read (@"D:\brisi\austria-latest.osm.pbf", fileSystem, analyzer);
            }
        }
    }
}
