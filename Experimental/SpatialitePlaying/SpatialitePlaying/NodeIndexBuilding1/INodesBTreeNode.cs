namespace SpatialitePlaying.NodeIndexBuilding1
{
    public interface INodesBTreeNode
    {
        long StartNodeId { get; }
        NodesBlock FindNodeBlock(long nodeId);
    }
}