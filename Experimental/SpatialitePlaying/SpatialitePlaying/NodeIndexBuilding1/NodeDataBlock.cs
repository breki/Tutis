using System;

namespace SpatialitePlaying.NodeIndexBuilding1
{
    public class NodeDataBlock
    {
        public NodeDataBlock(int nodesCount)
        {
            if (nodesCount == 0)
                throw new ArgumentOutOfRangeException ("nodesCount", "nodesCount == 0");

            ids = new long[nodesCount];
            xs = new double[nodesCount];
            ys = new double[nodesCount];
        }

        public NodeData GetNodeData (long nodeId)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                long indexedNodeId = ids[i];
                if (indexedNodeId == nodeId)
                    return new NodeData(ids[i], xs[i], ys[i]);

                if (indexedNodeId > nodeId)
                    throw new InvalidOperationException("The node does not belong to this block");
            }

            throw new InvalidOperationException ("The node was not found");
        }

        public void SetNodeData (int i, long id, double x, double y)
        {
            ids[i] = id;
            xs[i] = x;
            ys[i] = y;
        }

        private long[] ids;
        private double[] xs;
        private double[] ys;
    }
}