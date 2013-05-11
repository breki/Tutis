namespace SpatialitePlaying.NodeIndexBuilding1.NodesStorage
{
    public interface INodesBTreeNode
    {
        long StartNodeId { get; }
        NodesBTreeLeafNode FindNodeBlock(long nodeId);
    }
}