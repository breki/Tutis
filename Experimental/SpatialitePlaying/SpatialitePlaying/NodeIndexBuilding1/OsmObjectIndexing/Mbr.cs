using System;
using System.IO;

namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public class Mbr
    {
        public long MinX
        {
            get { return minX; }
        }

        public long MinY
        {
            get { return minY; }
        }

        public long MaxX
        {
            get { return maxX; }
        }

        public long MaxY
        {
            get { return maxY; }
        }

        public void ExtendToCover(long x, long y)
        {
            minX = Math.Min(minX, x);
            minY = Math.Min(minY, y);
            maxX = Math.Max(maxX, x);
            maxY = Math.Max(maxY, y);
        }

        public void ExtendToCover (Mbr otherMbr)
        {
            minX = Math.Min (minX, otherMbr.minX);
            minY = Math.Min (minY, otherMbr.minY);
            maxX = Math.Max (maxX, otherMbr.maxX);
            maxY = Math.Max (maxY, otherMbr.maxY);
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