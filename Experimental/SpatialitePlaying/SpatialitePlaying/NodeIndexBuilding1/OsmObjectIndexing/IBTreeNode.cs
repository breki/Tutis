using System;

namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public interface IBTreeNode
    {
        long StartObjectId { get; }
        bool IsLeaf { get; }

        void VisitAllNodes(Action<IBTreeNode> visitAction);
        BTreeLeafNode FindLeafNode(long objectId);
    }
}