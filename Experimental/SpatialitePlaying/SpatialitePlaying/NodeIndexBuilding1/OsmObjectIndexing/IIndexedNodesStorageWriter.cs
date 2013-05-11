namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public interface IIndexedNodesStorageWriter : IIndexedOsmObjectStorageWriter
    {
        void StoreNode (long nodeId, double x, double y);
    }
}