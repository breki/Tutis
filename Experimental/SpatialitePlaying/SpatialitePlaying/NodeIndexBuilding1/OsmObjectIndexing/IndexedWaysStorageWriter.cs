using System;
using System.Collections.Generic;
using System.IO;
using Brejc.Common.BinaryProcessing;
using Brejc.Common.FileSystem;
using Brejc.Geometry;
using Brejc.OsmLibrary;
using SpatialitePlaying.NodeIndexBuilding1.RTrees;

namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public class IndexedWaysStorageWriter : IndexedOsmObjectStorageWriterBase, IIndexedWaysStorageWriter
    {
        public IndexedWaysStorageWriter (IFileSystem fileSystem) : base(fileSystem)
        {
        }

        public override void InitializeStorage (string storageName)
        {
            base.InitializeStorage (storageName);
            rtreeConstructor = new NaiveRTreeConstructor(FileSystem);
            rtreeConstructor.InitializeStorage(storageName, ObjectTypeName);
        }

        public void StoreWay(OsmWay way, IPointD2List points)
        {
            FlushCurrentBlockIfFull(way.ObjectId);

            Writer.Write (way.ObjectId);
            Mbr mbr;
            byte[] pointsBlob = PointsToBlob2(points, 1000, true, out mbr);
            mbr.WriteToStream(Writer);
            int blobLength = pointsBlob.Length;
            Writer.Write(blobLength);
            Writer.Write(pointsBlob);
            
            IncrementObjectsInBlockCount ();

            rtreeConstructor.AddObject(way.ObjectId, mbr);
        }

        public override void FinalizeStorage ()
        {
            base.FinalizeStorage ();

            rtreeConstructor.ConstructRTree();
        }

        private static byte[] PointsToBlob2 (IPointD2List points, int granularity, bool skipLastPoint, out Mbr wayMbr)
        {
            double resolution = granularity * OsmObjectIndexingHelper.Nanodegree;

            List<long> coordDiffs = new List<long> ();
            int pointsCount = points.PointsCount + (skipLastPoint ? -1 : 0);

            long lxi = 0;
            long lyi = 0;

            wayMbr = new Mbr();
            for (int i = 0; i < pointsCount; i++)
            {
                double x;
                double y;
                points.GetPoint (i, out x, out y);

                long xi = (long)Math.Round (x / resolution);
                long yi = (long)Math.Round (y / resolution);

                if (xi == lxi && yi == lyi)
                    continue;

                wayMbr.ExtendToCover(xi, yi);

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

        private IRTreeConstructor rtreeConstructor;
    }
}