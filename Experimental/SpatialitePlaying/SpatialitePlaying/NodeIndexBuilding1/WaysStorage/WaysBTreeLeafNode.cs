namespace SpatialitePlaying.NodeIndexBuilding1.WaysStorage
{
    public class WaysBTreeLeafNode : IWaysBTreeNode
    {
        public WaysBTreeLeafNode (long startNodeId, long filePosition)
        {
            this.startNodeId = startNodeId;
            this.filePosition = filePosition;
        }

        public long StartNodeId
        {
            get { return startNodeId; }
        }

        public long FilePosition
        {
            get { return filePosition; }
        }

        public int NodesCount
        {
            get { return nodesCount; }
            set { nodesCount = value; }
        }

        public long? NextBlockStartNodeId
        {
            get
            {
                if (nextBlockNodeIdOffset == 0)
                    return null;

                return startNodeId + nextBlockNodeIdOffset;
            }
        }

        public WaysBTreeLeafNode FindBlock (long wayId)
        {
            return this;
        }

        public void SetNextBlockNodeId (long nodeId)
        {
            nextBlockNodeIdOffset = (int)(nodeId - startNodeId);
        }

        private long startNodeId;
        private long filePosition;
        private int nodesCount;
        private int nextBlockNodeIdOffset;
    }
}