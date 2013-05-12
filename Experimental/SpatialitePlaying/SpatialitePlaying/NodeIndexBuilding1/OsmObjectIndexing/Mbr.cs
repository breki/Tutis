using System;
using System.IO;
using Brejc.Geometry;

namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public class Mbr
    {
        public Mbr()
        {
        }

        public Mbr (Bounds2 bounds, int granularity)
        {
            double resolution = granularity * OsmObjectIndexingHelper.Nanodegree;

            minX = (long)Math.Round(bounds.MinX/resolution);
            minY = (long)Math.Round(bounds.MinY/resolution);
            maxX = (long)Math.Round(bounds.MaxX/resolution);
            maxY = (long)Math.Round(bounds.MaxY/resolution);
        }

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

        public bool IntersectsWith (Mbr other)
        {
            if (maxX < other.minX) 
                return false; // a is left of b
            if (minX > other.maxX) 
                return false; // a is right of b
            if (maxY < other.minY) 
                return false; // a is above b
            if (minY > other.maxY) 
                return false; // a is below b

            return true;

            //return !(MinY > other.maxY || MaxY < other.minY || MaxX < other.minX || MinX > other.maxX);
        }

        public bool Contains (Mbr other)
        {
            return MathExt.IsBetween (minX, maxX, other.minX)
                   && MathExt.IsBetween (minX, maxX, other.maxX)
                   && MathExt.IsBetween (minY, maxY, other.minY)
                   && MathExt.IsBetween (minY, maxY, other.maxY);
        }

        public static Mbr ReadFromStream (BinaryReader reader)
        {
            Mbr mbr = new Mbr();
            mbr.minX = reader.ReadInt64();
            mbr.maxX = reader.ReadInt64();
            mbr.minY = reader.ReadInt64();
            mbr.maxY = reader.ReadInt64();
            return mbr;
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