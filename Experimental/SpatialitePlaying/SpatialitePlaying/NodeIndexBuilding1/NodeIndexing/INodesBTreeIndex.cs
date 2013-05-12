using System.Collections.Generic;
using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1.NodeIndexing
{
    public interface INodesBTreeIndex : IOsmObjectBTreeIndex
    {
        IDictionary<long, NodeData> FetchNodes (IEnumerable<long> nodeIds);
    }
}