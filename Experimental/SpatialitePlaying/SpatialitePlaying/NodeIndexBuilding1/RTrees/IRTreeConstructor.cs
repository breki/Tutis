using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1.RTrees
{
    public interface IRTreeConstructor
    {
        void InitializeStorage(string storageName, string objectTypeName);
        void AddObject(long objectId, Mbr objectMbr);
        void ConstructRTree();
    }
}