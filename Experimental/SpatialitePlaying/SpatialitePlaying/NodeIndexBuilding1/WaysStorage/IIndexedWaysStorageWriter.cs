using Brejc.Geometry;
using Brejc.OsmLibrary;

namespace SpatialitePlaying.NodeIndexBuilding1.WaysStorage
{
    public interface IIndexedWaysStorageWriter
    {
        void InitializeStorage();
        void StoreWay (OsmWay way, IPointD2List points);
        void FinalizeStorage();
    }
}