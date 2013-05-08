using System.Collections.Generic;
using System.IO;
using Brejc.Common.FileSystem;
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
            stream = fileSystem.OpenFileToWrite("nodes.dat");
            writer = new BinaryWriter(stream);

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
            if (nodesInBlockCounter%100 == 0)
            {
                NodesBlock block = new NodesBlock(node.ObjectId, stream.Position);
                blocks.Add(block);
                nodesInBlockCounter = 0;
            }

            writer.Write(node.ObjectId);
            writer.Write(node.X);
            writer.Write(node.Y);

            nodesInBlockCounter++;
        }

        public void AddWay (OsmWay way)
        {
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
        private Stream stream;
        private BinaryWriter writer;
        private List<NodesBlock> blocks = new List<NodesBlock>();
        private int nodesInBlockCounter;
    }
}