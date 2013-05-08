namespace SpatialitePlaying.NodeIndexBuilding1
{
    public class NodesBTreeNonLeafNone : INodesBTreeNode
    {
        public NodesBTreeNonLeafNone(INodesBTreeNode leftNode, INodesBTreeNode rightNode)
        {
            this.leftNode = leftNode;
            this.rightNode = rightNode;
            leftNodeStartId = leftNode.StartNodeId;
            rightNodeStartId = rightNode.StartNodeId;
        }

        public long StartNodeId { get { return leftNodeStartId; } }

        public NodesBlock FindNodeBlock (long nodeId)
        {
            if (nodeId < rightNodeStartId)
                return leftNode.FindNodeBlock(nodeId);

            return rightNode.FindNodeBlock(nodeId);
        }

        private INodesBTreeNode leftNode;
        private INodesBTreeNode rightNode;
        private readonly long leftNodeStartId;
        private readonly long rightNodeStartId;
    }
}