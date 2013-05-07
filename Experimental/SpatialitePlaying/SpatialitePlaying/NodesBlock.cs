namespace SpatialitePlaying
{
    public class NodesBlock
    {
        public NodesBlock(long startNodeId, long filePosition)
        {
            this.startNodeId = startNodeId;
            this.filePosition = filePosition;
        }

        private long startNodeId;
        private long filePosition;
    }
}