using Brejc.Geometry;
using Brejc.OsmLibrary;

namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public interface IIndexedWaysStorageWriter : IIndexedOsmObjectStorageWriter
    {
        void StoreWay (OsmWay way, IPointD2List points);
    }
}