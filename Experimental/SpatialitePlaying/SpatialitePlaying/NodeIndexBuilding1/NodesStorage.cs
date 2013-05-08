using System.Collections.Generic;
using System.IO;
using Brejc.Common.FileSystem;
using Brejc.Geometry;

namespace SpatialitePlaying.NodeIndexBuilding1
{
    public class NodesStorage : INodesStorage
    {
        public NodesStorage(string storageFileName, IFileSystem fileSystem)
        {
            this.storageFileName = storageFileName;
            this.fileSystem = fileSystem;
        }

        public void InitializeForWriting()
        {
            stream = fileSystem.OpenFileToWrite (storageFileName);
            writer = new BinaryWriter (stream);
        }

        public void WriteNode(long nodeId, double x, double y)
        {
            if (nodesInBlockCounter % 100 == 0)
            {
                if (previousBlock != null)
                    previousBlock.SetNextBlockNodeId(nodeId);

                NodesBlock block = new NodesBlock (nodeId, stream.Position);
                blocks.Add (block);
                nodesInBlockCounter = 0;
                previousBlock = block;
            }

            writer.Write (nodeId);
            writer.Write (x);
            writer.Write (y);

            nodesInBlockCounter++;
        }

        public void CloseForWriting()
        {
            writer.Close();
            writer.Dispose();
            writer = null;
            stream.Close();
            stream.Dispose();
            stream = null;

            ConstructBTree ();
        }

        public void InitializeForReading()
        {
            stream = fileSystem.OpenFileToRead(storageFileName);
            reader = new BinaryReader(stream);
        }

        public IDictionary<long, PointD2> FetchNodes(IEnumerable<long> nodeIds)
        {
            Dictionary<long, PointD2> nodesData = new Dictionary<long, PointD2>();
            
            NodesBlock currentBlock = null;
            foreach (long nodeId in nodeIds)
            {
                if (currentBlock == null)
                    currentBlock = btree.FindNodeBlock(nodeId);

                if (nodeId >= currentBlock.NextBlockStartNodeId)
                    currentBlock = btree.FindNodeBlock (nodeId);
            }
            throw new System.NotImplementedException();
        }

        public void CloseForReading()
        {
            reader.Close ();
            reader.Dispose ();
            reader = null;
            stream.Close ();
            stream.Dispose ();
            stream = null;
        }

        private void ConstructBTree()
        {
            List<INodesBTreeNode> upperLevel = new List<INodesBTreeNode>();
            List<INodesBTreeNode> lowerLevel = new List<INodesBTreeNode>();

            lowerLevel.AddRange (blocks);

            while (lowerLevel.Count >= 2)
            {
                for (int i = 0; i < lowerLevel.Count; i += 2)
                {
                    INodesBTreeNode node1 = lowerLevel[i];

                    // move the odd one to the higher level
                    if (i + 1 == lowerLevel.Count)
                        upperLevel.Add(node1);
                    else
                    {
                        INodesBTreeNode node2 = lowerLevel[i + 1];
                        NodesBTreeNonLeafNone parentNode = new NodesBTreeNonLeafNone(
                            node1, node2);
                        upperLevel.Add(parentNode);
                    }
                }

                lowerLevel.Clear();
                lowerLevel = upperLevel;
                upperLevel = new List<INodesBTreeNode>();
            }

            btree = lowerLevel[0];
        }

        private readonly string storageFileName;
        private readonly IFileSystem fileSystem;
        private Stream stream;
        private BinaryWriter writer;
        private BinaryReader reader;
        private int nodesInBlockCounter;
        private List<NodesBlock> blocks = new List<NodesBlock> ();
        private INodesBTreeNode btree;
        private NodesBlock previousBlock;
    }
}