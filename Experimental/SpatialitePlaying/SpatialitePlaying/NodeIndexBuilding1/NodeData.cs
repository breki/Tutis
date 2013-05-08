namespace SpatialitePlaying.NodeIndexBuilding1
{
    public class NodeData
    {
        public NodeData(long nodeId, double x, double y)
        {
            this.nodeId = nodeId;
            this.x = x;
            this.y = y;
        }

        public long NodeId
        {
            get { return nodeId; }
        }

        public double X
        {
            get { return x; }
        }

        public double Y
        {
            get { return y; }
        }

        private long nodeId;
        private double x;
        private double y;
    }
}