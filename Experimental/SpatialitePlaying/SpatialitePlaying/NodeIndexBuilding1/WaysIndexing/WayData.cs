using System;
using System.IO;
using Brejc.Common.BinaryProcessing;
using Brejc.Geometry;
using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1.WaysIndexing
{
    public class WayData
    {
        public WayData(long wayId, Mbr mbr, short category, byte[] pointsBlob)
        {
            this.wayId = wayId;
            this.mbr = mbr;
            this.category = category;
            this.pointsBlob = pointsBlob;
        }

        public long WayId
        {
            get { return wayId; }
        }

        public Mbr Mbr
        {
            get { return mbr; }
        }

        public short Category
        {
            get { return category; }
        }

        public byte[] PointsBlob
        {
            get { return pointsBlob; }
        }

        public IPointD2List GetPointsList ()
        {
            double resolution = 1000 * OsmObjectIndexingHelper.Nanodegree;
            bool duplicateFirstPointAsLast = true;

            using (MemoryStream stream = new MemoryStream (pointsBlob))
            using (BinaryReader binaryReader = new BinaryReader (stream))
            {
                int pointsInDbCount = (int)binaryReader.ReadVarintUInt64 ();
                int totalPointsCount = pointsInDbCount + (duplicateFirstPointAsLast ? 1 : 0);
                PointD2Array points = new PointD2Array (totalPointsCount);

                long lxi = 0;
                long lyi = 0;

                for (int i = 0; i < pointsInDbCount; i++)
                {
                    long xi = binaryReader.ReadVarintInt64 ();
                    long yi = binaryReader.ReadVarintInt64 ();

                    if (xi == 0 && yi == 0)
                        throw new InvalidOperationException ("Skip duplicate points");

                    double x = (xi + lxi) * resolution;
                    double y = (yi + lyi) * resolution;
                    points.SetPoint (i, x, y);

                    lxi = xi + lxi;
                    lyi = yi + lyi;
                }

                if (duplicateFirstPointAsLast)
                    points.SetPoint (totalPointsCount - 1, points.GetPointD2 (0));

                return points;
            }
        }

        private long wayId;
        private Mbr mbr;
        private short category;
        private byte[] pointsBlob;
    }
}