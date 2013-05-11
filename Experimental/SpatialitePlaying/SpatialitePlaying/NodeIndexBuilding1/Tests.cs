using Brejc.Common.FileSystem;
using SpatialitePlaying.CustomPbf;
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
            using (OsmPbfReader2 osmReader = new OsmPbfReader2 ())
            {
                osmReader.Settings.SkipRelations = true;
                osmReader.Settings.SkipWays = false;

                //osmReader.Read (@"D:\brisi\isle-of-man-latest.osm.pbf", fileSystem, osmDb);
                //osmReader.Read (@"D:\brisi\slovenia-latest.osm.pbf", fileSystem, recorder);
                //osmReader.Read (@"D:\brisi\austria-latest.osm.pbf", fileSystem, processor);
                osmReader.Read (@"D:\brisi\germany-latest.osm.pbf", fileSystem, processor);
            }
        }
    }
}
