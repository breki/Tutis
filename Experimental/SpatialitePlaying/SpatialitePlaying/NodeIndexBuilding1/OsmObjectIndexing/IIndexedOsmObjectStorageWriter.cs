namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public interface IIndexedOsmObjectStorageWriter
    {
        void InitializeStorage(string storageName);
        void FinalizeStorage();
    }
}