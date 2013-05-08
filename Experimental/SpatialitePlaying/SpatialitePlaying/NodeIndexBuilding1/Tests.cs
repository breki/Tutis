using Brejc.Common.FileSystem;
using Brejc.OsmLibrary.Pbf;
using NUnit.Framework;

namespace SpatialitePlaying.NodeIndexBuilding1
{
    public class Tests
    {
        [Test]
        public void StoreNodesInBinaryFile()
        {
            OsmFileProcessor processor = new OsmFileProcessor(new WindowsFileSystem());

            WindowsFileSystem fileSystem = new WindowsFileSystem ();
            using (OsmPbfReader osmReader = new OsmPbfReader ())
            {
                osmReader.Settings.SkipRelations = true;
                osmReader.Settings.SkipWays = false;
                osmReader.Settings.IgnoreCreatedByTags = true;
                osmReader.Settings.LoadExtendedData = false;

                //osmReader.Read (@"D:\brisi\isle-of-man-latest.osm.pbf", fileSystem, osmDb);
                //osmReader.Read (@"D:\brisi\slovenia-latest.osm.pbf", fileSystem, recorder);
                osmReader.Read (@"D:\brisi\austria-latest.osm.pbf", fileSystem, processor);
            }
        }
    }
}
