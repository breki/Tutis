using System.Collections.Generic;
using Brejc.Geometry;

namespace SpatialitePlaying.NodeIndexBuilding1
{
    public interface INodesStorage
    {
        void InitializeForWriting();
        void WriteNode(long nodeId, double x, double y);
        void CloseForWriting();

        void InitializeForReading();
        IDictionary<long, PointD2> FetchNodes(IEnumerable<long> nodeIds);
        void CloseForReading ();
    }
}