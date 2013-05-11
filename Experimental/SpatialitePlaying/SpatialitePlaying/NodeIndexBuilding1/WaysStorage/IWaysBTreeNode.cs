namespace SpatialitePlaying.NodeIndexBuilding1.WaysStorage
{
    public interface IWaysBTreeNode
    {
        long StartNodeId { get; }
        WaysBTreeLeafNode FindBlock(long wayId);
    }
}