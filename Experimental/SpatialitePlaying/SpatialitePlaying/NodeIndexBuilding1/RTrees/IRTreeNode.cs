using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1.RTrees
{
    public interface IRTreeNode
    {
        Mbr Mbr { get; }
        bool IsLeaf { get; }
    }
}