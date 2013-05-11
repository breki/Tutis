namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public class BTreeNonLeafNode : IBTreeNode
    {
        public BTreeNonLeafNode (IBTreeNode leftNode, IBTreeNode rightNode)
        {
            this.leftNode = leftNode;
            this.rightNode = rightNode;
            leftNodeStartId = leftNode.StartObjectId;
            rightNodeStartId = rightNode.StartObjectId;
        }

        public long StartObjectId { get { return leftNodeStartId; } }

        public BTreeLeafNode FindBlock (long objectId)
        {
            if (objectId < rightNodeStartId)
                return leftNode.FindBlock(objectId);

            return rightNode.FindBlock(objectId);
        }

        private IBTreeNode leftNode;
        private IBTreeNode rightNode;
        private readonly long leftNodeStartId;
        private readonly long rightNodeStartId;
    }
}