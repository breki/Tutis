using System;
using System.Collections.Generic;
using Brejc.Geometry;
using Brejc.OsmLibrary;

namespace SpatialitePlaying
{
    public class TestOsmDataStorage : IOsmDataStorage
    {
        public string AttributionText { get; set; }

        public ReadingPhase ReadingPhase
        {
            get { return readingPhase; }
            set { readingPhase = value; }
        }

        public IOsmDataBulkInsertSession StartBulkInsertSession(bool threadSafe)
        {
            return new TestOsmDataBulkInsertSession(this, readingPhase);
        }

        public void EndBulkInsertSession()
        {
            readingPhase = readingPhase + 1;
        }

        public HashSet<long> UsedNodes
        {
            get { return usedNodes; }
        }

        public List<OsmWay> UsedWays
        {
            get { return usedWays; }
        }

        public void AddNodeData (OsmNode node)
        {
            nodesData[node.ObjectId] = new PointD2(node.X, node.Y);
        }

        public IPointD2 GetNodeData(long nodeId)
        {
            PointD2 value;
            if (!nodesData.TryGetValue(nodeId, out value))
                return null;

            return value;
        }

        private ReadingPhase readingPhase = ReadingPhase.ReadWays;
        private HashSet<long> usedNodes = new HashSet<long> ();
        private Dictionary<long, PointD2> nodesData = new Dictionary<long, PointD2> ();
        private List<OsmWay> usedWays = new List<OsmWay> ();
    }
}