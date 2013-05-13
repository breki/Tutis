using System.Collections.Generic;
using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1.WaysIndexing
{
    public interface IWaysBTreeIndex : IOsmObjectBTreeIndex
    {
        IDictionary<long, WayData> FetchWays(IEnumerable<long> ids);
    }
}