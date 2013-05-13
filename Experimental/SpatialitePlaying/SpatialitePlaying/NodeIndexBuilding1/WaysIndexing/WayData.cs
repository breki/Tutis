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

        private long wayId;
        private Mbr mbr;
        private short category;
        private byte[] pointsBlob;
    }
}