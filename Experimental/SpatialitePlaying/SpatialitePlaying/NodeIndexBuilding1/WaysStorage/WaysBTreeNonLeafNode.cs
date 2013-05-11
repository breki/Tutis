namespace SpatialitePlaying.NodeIndexBuilding1.WaysStorage
{
    public class WaysBTreeNonLeafNode : IWaysBTreeNode
    {
        public WaysBTreeNonLeafNode (IWaysBTreeNode leftNode, IWaysBTreeNode rightNode)
        {
            this.leftNode = leftNode;
            this.rightNode = rightNode;
            leftNodeStartId = leftNode.StartNodeId;
            rightNodeStartId = rightNode.StartNodeId;
        }

        public long StartNodeId { get { return leftNodeStartId; } }

        public WaysBTreeLeafNode FindBlock (long wayId)
        {
            if (wayId < rightNodeStartId)
                return leftNode.FindBlock(wayId);

            return rightNode.FindBlock(wayId);
        }

        private IWaysBTreeNode leftNode;
        private IWaysBTreeNode rightNode;
        private readonly long leftNodeStartId;
        private readonly long rightNodeStartId;
    }
}