using System;
using System.Collections.Generic;
using System.Diagnostics;
using Brejc.Common.FileSystem;
using Brejc.Geometry;
using Brejc.OsmLibrary;
using SpatialitePlaying.CustomPbf;
using SpatialitePlaying.NodeIndexBuilding1.NodesStorage;
using SpatialitePlaying.NodeIndexBuilding1.WaysStorage;

namespace SpatialitePlaying.NodeIndexBuilding1
{
    public class OsmFileProcessor : IOsmObjectDiscovery
    {
        public OsmFileProcessor(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public string AttributionText { get; set; }

        public void Begin()
        {
            Console.WriteLine ("Started reading OSM data...");

            nodesStorage = new NodesStorage.NodesStorage("nodes.dat", fileSystem);
            nodesStorage.InitializeForWriting();
            waysStorage = new IndexedWaysStorageWriter("ways.dat", fileSystem);
            waysStorage.InitializeStorage();

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
            nodesStorage.WriteNode(node.ObjectId, node.X, node.Y);
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
            if (!finishedReadingNodes)
            {
                nodesStorage.CloseForWriting();
                nodesStorage.InitializeForReading();
                finishedReadingNodes = true;
                Console.WriteLine("Started reading ways...");
                stopwatch.Start();
            }

            if (way.IsClosed)
                return;

            cachedWays.Add(way);

            if (cachedWays.Count > 10000)
            {
                SortedSet<long> neededNodes = new SortedSet<long>();
                foreach (OsmWay cachedWay in cachedWays)
                {
                    foreach (long nodeId in cachedWay.Nodes)
                    {
                        if (nodeId < 0)
                            Debugger.Break();

                        neededNodes.Add(nodeId);
                    }
                }

                IDictionary<long, NodeData> nodesDict = nodesStorage.FetchNodes(neededNodes);

                waysCount += cachedWays.Count;

                foreach (OsmWay cachedWay in cachedWays)
                {
                    PointD2List points = new PointD2List(way.NodesCount);
                    foreach (long nodeId in cachedWay.Nodes)
                    {
                        NodeData node = nodesDict[nodeId];
                        points.AddPoint(node.X, node.Y);
                    }

                    waysStorage.StoreWay(cachedWay, points);
                    waysCount++;
                }

                cachedWays.Clear();

                long elapsedSeconds = stopwatch.ElapsedMilliseconds / 1000;
                Console.WriteLine (
                    "Added {0} ways in {1} s ({2} ways/s)",
                    waysCount,
                    elapsedSeconds,
                    ((double)waysCount) / elapsedSeconds);
            }
        }

        public void ProcessRelation (OsmRelation relation)
        {
        }

        public void ProcessBoundingBox (OsmBoundingBox box)
        {
        }

        private readonly IFileSystem fileSystem;
        private List<OsmWay> cachedWays = new List<OsmWay>();
        private int nodesCount;
        private int waysCount;
        private INodesStorage nodesStorage;
        private IIndexedWaysStorageWriter waysStorage;
        private bool finishedReadingNodes;
        private Stopwatch stopwatch = new Stopwatch();
    }
}