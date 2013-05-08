namespace SpatialitePlaying.NodeIndexBuilding1
{
    public interface INodesBTreeNode
    {
        long StartNodeId { get; }
        NodesBTreeLeafNode FindNodeBlock(long nodeId);
    }
}