using System;
using Brejc.OsmLibrary;

namespace SpatialitePlaying
{
    public class TestOsmDataBulkInsertSession : IOsmDataBulkInsertSession
    {
        public TestOsmDataBulkInsertSession(TestOsmDataStorage storage, ReadingPhase readingPhase)
        {
            this.storage = storage;
            this.readingPhase = readingPhase;
        }

        public void AddBoundingBox(OsmBoundingBox box)
        {
        }

        public void AddNode(OsmNode node)
        {
            if (storage.UsedNodes.Contains(node.ObjectId))
                storage.AddNodeData(node);
        }

        public void AddNote(string noteType, string note)
        {
        }

        public void AddRelation(OsmRelation relation)
        {
        }

        public void AddWay(OsmWay way)
        {
            if (!way.IsClosed)
                return;

            storage.UsedWays.Add(way);
            foreach (long nodeId in way.Nodes)
                storage.UsedNodes.Add(nodeId);
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
    }
}