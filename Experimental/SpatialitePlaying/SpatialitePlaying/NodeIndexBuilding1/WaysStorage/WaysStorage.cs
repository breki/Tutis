using System.Collections.Generic;
using System.IO;
using Brejc.Common.FileSystem;
using Brejc.OsmLibrary;
using SpatialitePlaying.NodeIndexBuilding1.NodesStorage;

namespace SpatialitePlaying.NodeIndexBuilding1.WaysStorage
{
    public class WaysStorage : IWaysStorage
    {
        public WaysStorage (string storageFileName, IFileSystem fileSystem)
        {
            this.storageFileName = storageFileName;
            this.fileSystem = fileSystem;
        }

        public void InitializeForWriting()
        {
            stream = fileSystem.OpenFileToWrite (storageFileName);
            writer = new BinaryWriter (stream);
        }

        public void WriteWay(OsmWay way)
        {
            if (itemsInBlockCounter % 100 == 0)
            {
                if (previousBlock != null)
                    previousBlock.SetNextBlockNodeId (way.ObjectId);

                if (previousBlock != null)
                    previousBlock.NodesCount = itemsInBlockCounter;

                NodesBTreeLeafNode block = new NodesBTreeLeafNode (way.ObjectId, stream.Position);
                leafNodes.Add (block);
                itemsInBlockCounter = 0;
                previousBlock = block;
            }

            writer.Write (way.ObjectId);
            //writer.Write (x);
            //writer.Write (y);

            itemsInBlockCounter++;
        }

        public void CloseForWriting()
        {
            if (previousBlock != null)
                previousBlock.NodesCount = itemsInBlockCounter;

            writer.Close ();
            writer.Dispose ();
            writer = null;
            stream.Close ();
            stream.Dispose ();
            stream = null;

            ConstructBTree ();
        }

        private void ConstructBTree ()
        {
            //List<INodesBTreeNode> upperLevel = new List<INodesBTreeNode> ();
            //List<INodesBTreeNode> lowerLevel = new List<INodesBTreeNode> ();

            //lowerLevel.AddRange (leafNodes);

            //while (lowerLevel.Count >= 2)
            //{
            //    for (int i = 0; i < lowerLevel.Count; i += 2)
            //    {
            //        INodesBTreeNode node1 = lowerLevel[i];

            //        // move the odd one to the higher level
            //        if (i + 1 == lowerLevel.Count)
            //            upperLevel.Add (node1);
            //        else
            //        {
            //            INodesBTreeNode node2 = lowerLevel[i + 1];
            //            NodesBTreeNonLeafNode parentNode = new NodesBTreeNonLeafNode (
            //                node1, node2);
            //            upperLevel.Add (parentNode);
            //        }
            //    }

            //    lowerLevel.Clear ();
            //    lowerLevel = upperLevel;
            //    upperLevel = new List<INodesBTreeNode> ();
            //}

            //btree = lowerLevel[0];
            //leafNodes.Clear ();
        }

        private readonly string storageFileName;
        private readonly IFileSystem fileSystem;
        private Stream stream;
        private BinaryWriter writer;
        private int itemsInBlockCounter;
        private List<NodesBTreeLeafNode> leafNodes = new List<NodesBTreeLeafNode> ();
        //private INodesBTreeNode btree;
        private NodesBTreeLeafNode previousBlock;
        //private Dictionary<long, NodeDataBlock> nodeBlocks = new Dictionary<long, NodeDataBlock> ();
        //private int nodeBlocksCacheSize = 1000000;
    }
}