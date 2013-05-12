using System;
using System.IO;

namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public class Mbr
    {
        public void ExtendToCover(long x, long y)
        {
            minX = Math.Min(minX, x);
            minY = Math.Min(minY, y);
            maxX = Math.Max(maxX, x);
            maxY = Math.Max(maxY, y);
        }

        public void WriteToStream(BinaryWriter writer)
        {
            writer.Write(minX);
            writer.Write(maxX);
            writer.Write(minY);
            writer.Write(maxY);
        }

        private long minX = long.MaxValue, minY = long.MaxValue, maxX = long.MinValue, maxY = long.MinValue;
    }
}