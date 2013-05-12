using System.Collections.Generic;
using System.IO;
using System.Linq;
using Brejc.Common.FileSystem;
using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1.NodeIndexing
{
    public class NodesBTreeIndex : OsmObjectBTreeIndexBase, INodesBTreeIndex
    {
        public NodesBTreeIndex(IFileSystem fileSystem) : base(fileSystem)
        {
        }

        public int NodeBlocksCacheSize
        {
            get { return nodeBlocksCacheSize; }
            set { nodeBlocksCacheSize = value; }
        }

        public IDictionary<long, NodeData> FetchNodes (IEnumerable<long> nodeIds)
        {
            Dictionary<long, NodeData> nodesData = new Dictionary<long, NodeData> ();
            
            BTreeLeafNode currentLeafNode = null;
            int? startingIndex = null;
            foreach (long nodeId in nodeIds)
            {
                if (currentLeafNode == null || nodeId >= currentLeafNode.NextBlockStartObjectId)
                {
                    currentLeafNode = Btree.FindLeafNode(nodeId);
                    startingIndex = null;
                }

                NodeData nodeData = GetNodeData(nodeId, currentLeafNode, ref startingIndex);
                nodesData.Add(nodeId, nodeData);
            }

            return nodesData;
        }

        protected override string ObjectTypeName
        {
            get { return "nodes"; }
        }

        private NodeData GetNodeData(long nodeId, BTreeLeafNode treeLeaf, ref int? startingIndex)
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

            return nodeDataBlock.GetNodeData(nodeId, ref startingIndex);
        }

        private NodeDataBlock ReadNodeDataBlock(BTreeLeafNode treeLeaf)
        {
            DataReader.BaseStream.Seek(treeLeaf.FilePosition, SeekOrigin.Begin);

            NodeDataBlock block = new NodeDataBlock(treeLeaf.ObjectsCount);

            for (int i = 0; i < treeLeaf.ObjectsCount; i++)
            {
                long id = DataReader.ReadInt64 ();
                double x = DataReader.ReadDouble ();
                double y = DataReader.ReadDouble ();
                block.SetNodeData(i, id, x, y);
            }

            return block;
        }

        private Dictionary<long, NodeDataBlock> nodeBlocks = new Dictionary<long, NodeDataBlock>();
        private int nodeBlocksCacheSize = 200000;
    }
}