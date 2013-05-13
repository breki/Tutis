using System;
using System.Collections.Generic;
using System.Diagnostics;
using Brejc.Common.FileSystem;
using Brejc.Geometry;
using Brejc.OsmLibrary;
using SpatialitePlaying.CustomPbf;
using SpatialitePlaying.NodeIndexBuilding1.Features;
using SpatialitePlaying.NodeIndexBuilding1.NodeIndexing;
using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1
{
    public class OsmFileProcessor : IOsmObjectDiscovery
    {
        public OsmFileProcessor(AreaFeatures areaFeatures, IFileSystem fileSystem)
        {
            this.areaFeatures = areaFeatures;
            this.fileSystem = fileSystem;
        }

        public string AttributionText { get; set; }

        public void Begin()
        {
            Console.WriteLine ("Started reading OSM data...");

            nodesStorageWriter = new IndexedNodesStorageWriter(fileSystem);
            nodesStorageWriter.InitializeStorage(storageName);

            waysStorageWriter = new IndexedWaysStorageWriter(fileSystem);
            waysStorageWriter.InitializeStorage (storageName);

            stopwatch.Start ();
        }

        public void End ()
        {
        }

        public void Dispose ()
        {
        }

        public void ProcessNode (OsmNode node)
        {
            long objectId = node.ObjectId;

            if (objectId <= lastNodeId)
                throw new InvalidOperationException("Nodes in OSM file are not monotone.");

            lastNodeId = objectId;

            nodesStorageWriter.StoreNode(objectId, node.X, node.Y);
            nodesCount++;
            if (nodesCount%200000 == 0)
            {
                long elapsedSeconds = stopwatch.ElapsedMilliseconds / 1000;
                Console.WriteLine (
                    "Added {0} nodes in {1} s ({2} nodes/s)",
                    nodesCount,
                    elapsedSeconds,
                    ((double)nodesCount) / elapsedSeconds);
            }
        }

        public void ProcessWay (OsmWay way)
        {
            if (nodesBTreeIndex == null)
            {
                Console.WriteLine("Finalizing nodes storage writing...");
                nodesStorageWriter.FinalizeStorage();
                nodesStorageWriter = null;

                Console.WriteLine ("Preparing nodes b-tree...");
                nodesBTreeIndex = new NodesBTreeIndex(fileSystem);
                nodesBTreeIndex.Connect(storageName);

                Console.WriteLine("Started reading ways...");
                stopwatch.Start();
            }

            if (!way.IsClosed)
                return;

            short? category = MatchCategory(way);
            if (category == null)
                return;

            queuedWays.Add(new Tuple<OsmWay, short>(way, category.Value));

            if (queuedWays.Count > 10000)
                StoreQueuedWays();
        }

        public void ProcessRelation (OsmRelation relation)
        {
            if (waysStorageWriter != null)
            {
                if (queuedWays.Count > 0)
                    StoreQueuedWays();

                waysStorageWriter.FinalizeStorage();
                waysStorageWriter = null;
            }
        }

        private void StoreQueuedWays()
        {
            SortedSet<long> neededNodes = new SortedSet<long>();
            foreach (Tuple<OsmWay, short> queuedWay in queuedWays)
            {
                foreach (long nodeId in queuedWay.Item1.Nodes)
                    neededNodes.Add(nodeId);
            }

            IDictionary<long, NodeData> nodesDict = nodesBTreeIndex.FetchNodes(neededNodes);

            waysCount += queuedWays.Count;

            foreach (Tuple<OsmWay, short> queuedWay in queuedWays)
            {
                PointD2List points = new PointD2List (queuedWay.Item1.NodesCount);
                foreach (long nodeId in queuedWay.Item1.Nodes)
                {
                    NodeData node = nodesDict[nodeId];
                    points.AddPoint(node.X, node.Y);
                }

                waysStorageWriter.StoreWay(queuedWay.Item1, queuedWay.Item2, points);
                waysCount++;
            }

            queuedWays.Clear();

            long elapsedSeconds = stopwatch.ElapsedMilliseconds/1000;
            Console.WriteLine(
                "Added {0} ways in {1} s ({2} ways/s)",
                waysCount,
                elapsedSeconds,
                ((double)waysCount)/elapsedSeconds);
        }

        public void ProcessBoundingBox (OsmBoundingBox box)
        {
        }

        private short? MatchCategory(OsmWay way)
        {
            foreach (Tuple<short, string, string> categoryTuple in areaFeatures.Categories)
            {
                if (way.HasTag(categoryTuple.Item2, categoryTuple.Item3))
                    return categoryTuple.Item1;
            }

            return null;
        }

        private readonly AreaFeatures areaFeatures;
        private readonly IFileSystem fileSystem;
        private List<Tuple<OsmWay, short>> queuedWays = new List<Tuple<OsmWay, short>> ();
        private int nodesCount;
        private long lastNodeId = 0;
        private int waysCount;
        private IIndexedNodesStorageWriter nodesStorageWriter;
        private IIndexedWaysStorageWriter waysStorageWriter;
        private INodesBTreeIndex nodesBTreeIndex;
        private Stopwatch stopwatch = new Stopwatch();
        private string storageName = "experiment";
    }
}