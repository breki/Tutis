using System;
using System.Collections.Generic;
using Brejc.Common.FileSystem;
using Brejc.Geometry;
using SpatialitePlaying.CustomPbf;
using NUnit.Framework;
using SpatialitePlaying.NodeIndexBuilding1.Features;
using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;
using SpatialitePlaying.NodeIndexBuilding1.RTrees;
using SpatialitePlaying.NodeIndexBuilding1.WaysIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1
{
    public class RTreeTests
    {
        [Test]
        public void StoreNodesInBinaryFile()
        {
            AreaFeatures areaFeatures = new AreaFeatures();
            areaFeatures
                .AddCategory(1, "natural", "wood")
                .AddCategory(1, "landuse", "forest");

            OsmFileProcessor processor = new OsmFileProcessor(areaFeatures, new WindowsFileSystem());

            WindowsFileSystem fileSystem = new WindowsFileSystem ();
            using (OsmPbfReader2 osmReader = new OsmPbfReader2 ())
            {
                //osmReader.Settings.SkipRelations = true;

                //osmReader.Read (@"D:\brisi\isle-of-man-latest.osm.pbf", fileSystem, processor);
                osmReader.Read (@"D:\brisi\slovenia-latest.osm.pbf", fileSystem, processor);
                //osmReader.Read (@"D:\brisi\austria-latest.osm.pbf", fileSystem, processor);
                //osmReader.Read (@"D:\brisi\germany-latest.osm.pbf", fileSystem, processor);
            }
        }

        [Test]
        public void RunQuery()
        {
            ISpatialQuery spatialQuery = new SpatialQuery (new WindowsFileSystem ());
            Console.WriteLine ("Opening r-tree...");
            spatialQuery.Connect ("experiment", "ways");

            IWaysBTreeIndex waysIdIndex = new WaysBTreeIndex(new WindowsFileSystem());
            waysIdIndex.Connect("experiment");

            //Bounds2 bounds = new Bounds2 (15.4023050143043, 47.0599307403552, 15.4653307104719, 47.0833220386827);
            Bounds2 bounds = new Bounds2 (15.5438069890848, 46.5102421428415, 15.6504793872489, 46.5756640758013);
            Mbr queryMbr = new Mbr (bounds, 1000);

            Console.WriteLine ("Running r-tree query...");
            List<long> wayIds = spatialQuery.FindObjects (queryMbr);
            Console.WriteLine ("Result: {0} ways", wayIds.Count);

            wayIds.Sort((a, b) => a.CompareTo(b));
            IDictionary<long, WayData> waysData = waysIdIndex.FetchWays(wayIds);
        }
    }
}
