using System.Collections.Generic;

namespace SpatialitePlaying.NodeIndexBuilding1.NodesStorage
{
    public interface INodesStorage
    {
        void InitializeForWriting();
        void WriteNode(long nodeId, double x, double y);
        void CloseForWriting();

        void InitializeForReading();
        IDictionary<long, NodeData> FetchNodes (IEnumerable<long> nodeIds);
        void CloseForReading ();
    }
}