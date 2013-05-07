using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Brejc.OsmLibrary;
using Brejc.Rdbms;

namespace SpatialitePlaying
{
    public class TestOsmDataBulkInsertSession : IOsmDataBulkInsertSession
    {
        public TestOsmDataBulkInsertSession(TestOsmDataStorage storage, ReadingPhase readingPhase)
        {
            this.storage = storage;
            this.readingPhase = readingPhase;

            stopwatch.Start();
        }

        public void AddBoundingBox(OsmBoundingBox box)
        {
        }

        public void AddNode(OsmNode node)
        {
            if (readingPhase != ReadingPhase.ReadNodes)
                throw new InvalidOperationException ();

            //collectedNodes.Add(node);

            //if (collectedNodes.Count >= 100000)
            //{
            //    DBBulkProcessor.BulkUpdate(
            //        SqliteHelper.SqliteProviderFactory,
            //        storage.DbConnection,
            //        "SELECT node_id FROM nodes",
            //        "UPDATE nodes SET x=@p1, y=@p2 WHERE node_id=@p3",
            //        dt =>
            //        {
            //            foreach (OsmNode nodeToDb in collectedNodes)
            //            {
            //                DataRow row = dt.NewRow ();
            //                row[0] = nodeToDb.X;
            //                row[1] = nodeToDb.Y;
            //                row[2] = nodeToDb.ObjectId;
            //                dt.Rows.Add (row);
            //            }
            //        });

            //    nodesCounter += collectedNodes.Count;
            //    Console.WriteLine ("Nodes updated: {0} ({1} nodes/s)", nodesCounter, collectedNodes.Count / stopwatch.Elapsed.TotalSeconds);
            //    stopwatch.Restart ();

            //    collectedNodes.Clear ();
            //}
        }

        public void AddNote(string noteType, string note)
        {
        }

        public void AddRelation(OsmRelation relation)
        {
        }

        public void AddWay(OsmWay way)
        {
            if (readingPhase != ReadingPhase.ReadWays)
                throw new InvalidOperationException();

            if (!way.IsClosed)
                return;

            storage.UsedWays.Add(way);

            foreach (long nodeId in way.Nodes)
            {
                storage.NodesDict.Add(nodeId, true);
                cachedNodesCount++;
            }

            if (cachedNodesCount >= 100000)
            {
                storage.NodesStorage.Commit();

                nodesCounter += cachedNodesCount;
                Console.WriteLine("Nodes indexed: {0} ({1} nodes/s)", nodesCounter, cachedNodesCount / stopwatch.Elapsed.TotalSeconds);
                stopwatch.Restart();

                cachedNodesCount = 0;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources         

                if (disposing)
                {
                    // clean managed resources            
                }

                disposed = true;
            }
        }

        private bool disposed;
        private readonly TestOsmDataStorage storage;
        private readonly ReadingPhase readingPhase;
        private long cachedNodesCount;
        private List<OsmNode> collectedNodes = new List<OsmNode>();
        private long nodesCounter;
        private Stopwatch stopwatch = new Stopwatch();
    }
}