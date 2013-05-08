using System;
using System.Collections.Generic;
using System.IO;
using Brejc.Common.FileSystem;
using Brejc.Geometry;
using Brejc.OsmLibrary;

namespace SpatialitePlaying.NodeIndexBuilding1
{
    public class OsmNodesBinaryRecorder : IOsmDataStorage, IOsmDataBulkInsertSession
    {
        public OsmNodesBinaryRecorder(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public string AttributionText { get; set; }

        public IOsmDataBulkInsertSession StartBulkInsertSession (bool threadSafe)
        {
            nodesStorage = new NodesStorage("nodes.dat", fileSystem);
            nodesStorage.InitializeForWriting();
            return this;
        }

        public void EndBulkInsertSession ()
        {
        }

        public void Dispose ()
        {
        }

        public void AddNode (OsmNode node)
        {
            nodesStorage.WriteNode(node.ObjectId, node.X, node.Y);
        }

        public void AddWay (OsmWay way)
        {
            if (!initializedForReading)
            {
                nodesStorage.CloseForWriting();
                nodesStorage.InitializeForReading();
                initializedForReading = true;
                Console.WriteLine("Started reading ways...");
            }

            cachedWays.Add(way);

            if (cachedWays.Count > 10000)
            {
                SortedSet<long> neededNodes = new SortedSet<long>();
                foreach (OsmWay cachedWay in cachedWays)
                {
                    foreach (long nodeId in cachedWay.Nodes)
                        neededNodes.Add(nodeId);
                }

                IDictionary<long, PointD2> nodesDict = nodesStorage.FetchNodes(neededNodes);

                cachedWays.Clear();
            }
        }

        public void AddRelation (OsmRelation relation)
        {
        }

        public void AddBoundingBox (OsmBoundingBox box)
        {
        }

        public void AddNote (string noteType, string note)
        {
        }

        private readonly IFileSystem fileSystem;
        private List<OsmWay> cachedWays = new List<OsmWay>();
        private INodesStorage nodesStorage;
        private bool initializedForReading;
    }
}