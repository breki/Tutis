namespace SpatialitePlaying.NodeIndexBuilding1
{
    public class NodesBTreeLeafNode : INodesBTreeNode
    {
        public NodesBTreeLeafNode(long startNodeId, long filePosition)
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

        public NodesBTreeLeafNode FindNodeBlock (long nodeId)
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