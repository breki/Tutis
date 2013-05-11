using System.Collections.Generic;
using System.IO;
using System.Linq;
using Brejc.Common.FileSystem;

namespace SpatialitePlaying.NodeIndexBuilding1.NodesStorage
{
    public class NodesStorage : INodesStorage
    {
        public NodesStorage(string storageFileName, IFileSystem fileSystem)
        {
            this.storageFileName = storageFileName;
            this.fileSystem = fileSystem;
        }

        public int NodeBlocksCacheSize
        {
            get { return nodeBlocksCacheSize; }
            set { nodeBlocksCacheSize = value; }
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

                if (previousBlock != null)
                    previousBlock.NodesCount = nodesInBlockCounter;

                NodesBTreeLeafNode block = new NodesBTreeLeafNode (nodeId, stream.Position);
                leafNodes.Add (block);
                nodesInBlockCounter = 0;
                previousBlock = block;
                writer.Flush ();
            }

            writer.Write (nodeId);
            writer.Write (x);
            writer.Write (y);

            nodesInBlockCounter++;
        }

        public void CloseForWriting()
        {
            if (previousBlock != null)
                previousBlock.NodesCount = nodesInBlockCounter;

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

        public IDictionary<long, NodeData> FetchNodes (IEnumerable<long> nodeIds)
        {
            Dictionary<long, NodeData> nodesData = new Dictionary<long, NodeData> ();
            
            NodesBTreeLeafNode currentLeafNode = null;
            foreach (long nodeId in nodeIds)
            {
                if (currentLeafNode == null)
                    currentLeafNode = btree.FindNodeBlock(nodeId);

                if (nodeId >= currentLeafNode.NextBlockStartNodeId)
                    currentLeafNode = btree.FindNodeBlock (nodeId);

                NodeData nodeData = GetNodeData(nodeId, currentLeafNode);
                nodesData.Add(nodeId, nodeData);
            }

            return nodesData;
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

            lowerLevel.AddRange (leafNodes);

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
                        NodesBTreeNonLeafNode parentNode = new NodesBTreeNonLeafNode(
                            node1, node2);
                        upperLevel.Add(parentNode);
                    }
                }

                lowerLevel.Clear();
                lowerLevel = upperLevel;
                upperLevel = new List<INodesBTreeNode>();
            }

            btree = lowerLevel[0];
            leafNodes.Clear();
        }

        private NodeData GetNodeData(long nodeId, NodesBTreeLeafNode treeLeaf)
        {
            NodeDataBlock nodeDataBlock;

            if (!nodeBlocks.TryGetValue(treeLeaf.FilePosition, out nodeDataBlock))
            {
                nodeDataBlock = ReadNodeDataBlock(treeLeaf);
                //Console.WriteLine (nodeBlocks.Count);

                if (nodeBlocks.Count > nodeBlocksCacheSize)
                {
                    nodeBlocks.Remove(nodeBlocks.AsQueryable().First().Key);
                    //Console.WriteLine("XXX");
                }

                nodeBlocks.Add (treeLeaf.FilePosition, nodeDataBlock);
            }

            return nodeDataBlock.GetNodeData(nodeId);
        }

        private NodeDataBlock ReadNodeDataBlock(NodesBTreeLeafNode treeLeaf)
        {
            reader.BaseStream.Seek(treeLeaf.FilePosition, SeekOrigin.Begin);

            NodeDataBlock block = new NodeDataBlock(treeLeaf.NodesCount);

            for (int i = 0; i < treeLeaf.NodesCount; i++)
            {
                long id = reader.ReadInt64();
                double x = reader.ReadDouble();
                double y = reader.ReadDouble();
                block.SetNodeData(i, id, x, y);
            }

            return block;
        }

        private readonly string storageFileName;
        private readonly IFileSystem fileSystem;
        private Stream stream;
        private BinaryWriter writer;
        private BinaryReader reader;
        private int nodesInBlockCounter;
        private List<NodesBTreeLeafNode> leafNodes = new List<NodesBTreeLeafNode> ();
        private INodesBTreeNode btree;
        private NodesBTreeLeafNode previousBlock;
        private Dictionary<long, NodeDataBlock> nodeBlocks = new Dictionary<long, NodeDataBlock>();
        private int nodeBlocksCacheSize = 200000;
    }
}