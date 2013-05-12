using System;

namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public interface IOsmObjectBTreeIndex : IDisposable
    {
        void Connect(string storageName);
    }
}