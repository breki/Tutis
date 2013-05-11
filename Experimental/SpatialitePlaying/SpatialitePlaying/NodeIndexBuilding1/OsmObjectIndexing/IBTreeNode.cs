namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public interface IBTreeNode
    {
        long StartObjectId { get; }
        BTreeLeafNode FindBlock(long objectId);
    }
}