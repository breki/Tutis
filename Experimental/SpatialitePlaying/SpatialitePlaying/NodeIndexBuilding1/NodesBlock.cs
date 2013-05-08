namespace SpatialitePlaying.NodeIndexBuilding1
{
    public class NodesBlock : INodesBTreeNode
    {
        public NodesBlock(long startNodeId, long filePosition)
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

        public long? NextBlockStartNodeId
        {
            get
            {
                if (nextBlockNodeIdOffset == 0)
                    return null;

                return startNodeId + nextBlockNodeIdOffset;
            }
        }

        public NodesBlock FindNodeBlock (long nodeId)
        {
            return this;
        }

        public void SetNextBlockNodeId (long nodeId)
        {
            nextBlockNodeIdOffset = (int)(nodeId - startNodeId);
        }

        private long startNodeId;
        private long filePosition;
        private int nextBlockNodeIdOffset;
    }
}