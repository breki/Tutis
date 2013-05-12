using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1.RTrees
{
    public class RTreeObjectEntry
    {
        public RTreeObjectEntry (long objectId, Mbr mbr)
        {
            this.objectId = objectId;
            this.mbr = mbr;
        }

        public long ObjectId
        {
            get { return objectId; }
        }

        public Mbr Mbr
        {
            get { return mbr; }
        }

        private readonly long objectId;
        private readonly Mbr mbr;
    }
}