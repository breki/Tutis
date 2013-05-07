using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Brejc.Common.FileSystem;
using Brejc.Geometry;
using Brejc.OsmLibrary;
using Brejc.Rdbms;
using Sop;

namespace SpatialitePlaying
{
    public class TestOsmDataStorage : IOsmDataStorage
    {
        public TestOsmDataStorage(IFileSystem fileSystem)
        {
            Preferences preferences = new Preferences();
            nodesStorage = ObjectServer.OpenWithTransaction("nodes.dta", preferences);

            StoreFactory storeFactory = new StoreFactory();
            nodesDict = storeFactory.Get<long, bool> (nodesStorage.SystemFile.Store, "nodes");
        }

        public string AttributionText { get; set; }

        public ObjectServer NodesStorage
        {
            get { return nodesStorage; }
        }

        public ISortedDictionary<long, bool> NodesDict
        {
            get { return nodesDict; }
        }

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

        private readonly IFileSystem fileSystem;
        private ReadingPhase readingPhase = ReadingPhase.ReadWays;
        private Dictionary<long, PointD2> nodesData = new Dictionary<long, PointD2> ();
        private List<OsmWay> usedWays = new List<OsmWay> ();
        private ObjectServer nodesStorage;
        private ISortedDictionary<long, bool> nodesDict;
    }
}