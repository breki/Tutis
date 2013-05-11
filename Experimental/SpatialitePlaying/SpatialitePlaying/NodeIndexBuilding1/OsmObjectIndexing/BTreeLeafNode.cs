namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public class BTreeLeafNode : IBTreeNode
    {
        public BTreeLeafNode (long startObjectId, long filePosition)
        {
            this.startObjectId = startObjectId;
            this.filePosition = filePosition;
        }

        public long StartObjectId
        {
            get { return startObjectId; }
        }

        public long FilePosition
        {
            get { return filePosition; }
        }

        public int ObjectsCount
        {
            get { return objectsCount; }
            set { objectsCount = value; }
        }

        public long? NextBlockStartObjectId
        {
            get
            {
                if (nextBlockObjectIdOffset == 0)
                    return null;

                return startObjectId + nextBlockObjectIdOffset;
            }
        }

        public BTreeLeafNode FindBlock (long objectId)
        {
            return this;
        }

        public void SetNextBlockObjectId (long objectId)
        {
            nextBlockObjectIdOffset = (int)(objectId - startObjectId);
        }

        private long startObjectId;
        private long filePosition;
        private int objectsCount;
        private int nextBlockObjectIdOffset;
    }
}