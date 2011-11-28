using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Brejc.OsmLibrary;
using log4net;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace NoSqlPlaying
{
    public class MongoOsmDataBulkInsertSession : IOsmDataBulkInsertSession
    {
        public MongoOsmDataBulkInsertSession(MongoDatabase osmMongoDb)
        {
            this.osmMongoDb = osmMongoDb;

            osmMongoDb.DropCollection("nodes");
            nodes = osmMongoDb.GetCollection<Node>("nodes");
            nodes.EnsureIndex(IndexKeys.GeoSpatial("Loc"));
        }

        public void AddBoundingBox(OsmBoundingBox box)
        {
        }

        public void AddNode(OsmNode node)
        {
            nodesInBuffer.Add(node);
            if (nodesInBuffer.Count > 100000)
                BatchInsertNodes();
        }

        public void AddRelation(OsmRelation relation)
        {
        }

        public void AddWay(OsmWay way)
        {
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

        private void BatchInsertNodes()
        {
            int nodesCount = nodesInBuffer.Count;
            log.DebugFormat("Batch inserting {0} nodes", nodesCount);

            Node[] preparedMongoNodes = new Node[nodesCount];

            for (int i = 0; i < nodesCount; i++)
            {
                OsmNode node = nodesInBuffer[i];

                Node nodeMongo = new Node();
                nodeMongo.OsmId = node.ObjectId;
                //nodeMongo.Loc = new Point();
                //nodeMongo.Loc.X = node.X;
                //nodeMongo.Loc.Y = node.Y;

                //if (node.Tags != null)
                //{
                //    nodeMongo.Tags = new Hashtable();
                //    foreach (Tag tag in node.Tags)
                //        nodeMongo.Tags.Add(tag.Key, tag.Value);
                //}

                preparedMongoNodes[i] = nodeMongo;
            }

            Task insertTask = new Task(
                () =>
                    {
                        MongoInsertOptions options = new MongoInsertOptions(nodes);
                        options.CheckElementNames = false;
                        options.SafeMode = SafeMode.False;
                        nodes.InsertBatch(preparedMongoNodes, options);

                        log.Debug("Batch insert done");                        
                    });
            insertTask.Start();

            nodesInBuffer.Clear();
        }

        private bool disposed;
        private readonly MongoDatabase osmMongoDb;
        private MongoCollection<Node> nodes;
        private List<OsmNode> nodesInBuffer = new List<OsmNode>();
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}