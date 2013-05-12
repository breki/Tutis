using System;
using System.Collections.Generic;
using System.IO;
using Brejc.Common.BinaryProcessing;
using Brejc.Common.FileSystem;
using Brejc.Geometry;
using Brejc.OsmLibrary;

namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public class IndexedWaysStorageWriter : IndexedOsmObjectStorageWriterBase, IIndexedWaysStorageWriter
    {
        public IndexedWaysStorageWriter (IFileSystem fileSystem) : base(fileSystem)
        {
        }

        public void StoreWay(OsmWay way, IPointD2List points)
        {
            FlushCurrentBlockIfFull(way.ObjectId);

            Writer.Write (way.ObjectId);
            byte[] pointsBlob = PointsToBlob2(points, 1000, true);
            int blobLength = pointsBlob.Length;
            Writer.Write(blobLength);
            Writer.Write(pointsBlob);

            IncrementObjectsInBlockCount ();
        }

        private static byte[] PointsToBlob2 (IPointD2List points, int granularity, bool skipLastPoint)
        {
            const double Nanodegree = 0.000000001;
            double resolution = granularity * Nanodegree;

            List<long> coordDiffs = new List<long> ();
            int pointsCount = points.PointsCount + (skipLastPoint ? -1 : 0);

            long lxi = 0;
            long lyi = 0;

            for (int i = 0; i < pointsCount; i++)
            {
                double x;
                double y;
                points.GetPoint (i, out x, out y);

                long xi = (long)Math.Round (x / resolution);
                long yi = (long)Math.Round (y / resolution);

                if (xi == lxi && yi == lyi)
                    continue;

                coordDiffs.Add (xi - lxi);
                coordDiffs.Add (yi - lyi);

                lxi = xi;
                lyi = yi;
            }

            pointsCount = coordDiffs.Count / 2;

            byte[] data;
            using (MemoryStream stream = new MemoryStream ())
            using (BinaryWriter writer = new BinaryWriter (stream))
            {
                writer.WriteVarint ((ulong)pointsCount);

                foreach (long coordDiff in coordDiffs)
                    writer.WriteVarint (coordDiff);

                writer.Flush ();
                data = stream.ToArray ();
            }

            return data;
        }

        protected override string ObjectTypeName
        {
            get { return "ways"; }
        }
    }
}