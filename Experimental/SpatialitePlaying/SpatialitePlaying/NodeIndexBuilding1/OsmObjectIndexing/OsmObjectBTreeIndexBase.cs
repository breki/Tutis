using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Brejc.Common.FileSystem;

namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public abstract class OsmObjectBTreeIndexBase : IOsmObjectBTreeIndex
    {
        public void Connect(string storageName)
        {
            this.storageName = storageName;

            ReadBTreeIndex();
            ConnectToDataFile();
        }

        public static IBTreeNode ConstructBTreeFromLeafNodes(IList<BTreeLeafNode> leafNodes)
        {
            List<IBTreeNode> upperLevel = new List<IBTreeNode> ();
            List<IBTreeNode> lowerLevel = new List<IBTreeNode> ();

            lowerLevel.AddRange (leafNodes);

            while (lowerLevel.Count >= 2)
            {
                for (int i = 0; i < lowerLevel.Count; i += 2)
                {
                    IBTreeNode node1 = lowerLevel[i];

                    // move the odd one to the higher level
                    if (i + 1 == lowerLevel.Count)
                        upperLevel.Add (node1);
                    else
                    {
                        IBTreeNode node2 = lowerLevel[i + 1];
                        BTreeInnerNode parentNode = new BTreeInnerNode (
                            node1, node2);
                        upperLevel.Add (parentNode);
                    }

#if DEBUG
                    if (upperLevel.Count > 1)
                    {
                        long leftId = upperLevel[upperLevel.Count - 2].StartObjectId;
                        long rightId = upperLevel[upperLevel.Count - 1].StartObjectId;
                        if (leftId >= rightId)
                            throw new InvalidOperationException ("BUG: leftId >= rightId");
                    }
#endif
                }

                lowerLevel.Clear ();
                lowerLevel = upperLevel;
                upperLevel = new List<IBTreeNode> ();
            }

            IBTreeNode btree = lowerLevel[0];
            leafNodes.Clear ();

            return btree;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected OsmObjectBTreeIndexBase(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        protected abstract string ObjectTypeName { get; }

        public IBTreeNode Btree
        {
            get { return btree; }
        }

        public Stream DataStream
        {
            get { return dataStream; }
        }

        public BinaryReader DataReader
        {
            get { return dataReader; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources

                if (disposing)
                {
                    // clean managed resources           
                    btree = null;
                    if (dataReader != null)
                        dataReader.Dispose();
                    if (dataStream != null)
                        dataStream.Dispose();
                }

                disposed = true;
            }
        }

        private void ReadBTreeIndex ()
        {
            string dataFileName = string.Format (CultureInfo.InvariantCulture, "{0}-{1}-id.idx", storageName, ObjectTypeName);

            using (Stream btreeStream = fileSystem.OpenFileToRead (dataFileName))
            using (BinaryReader btreeReader = new BinaryReader (btreeStream))
            {
                btree = ReadBTree (btreeReader);
            }
        }

        private static IBTreeNode ReadBTree(BinaryReader btreeReader)
        {
            List<BTreeLeafNode> leafNodes = new List<BTreeLeafNode> ();

            BTreeLeafNode prevNode = null;
            while (true)
            {
                BTreeLeafNode nextNode = ReadBTreeLeafNode(btreeReader);
                if (nextNode == null)
                    break;

                leafNodes.Add(nextNode);
                if (prevNode != null)
                    prevNode.SetNextBlockObjectId (nextNode.StartObjectId);

                prevNode = nextNode;
            }

            return ConstructBTreeFromLeafNodes(leafNodes);
        }

        private static BTreeLeafNode ReadBTreeLeafNode(BinaryReader btreeReader)
        {
            long startObjectId = btreeReader.ReadInt64();

            if (startObjectId == 0)
                return null;

            long filePosition = btreeReader.ReadInt64();
            int objectsCount = btreeReader.ReadInt32();

            BTreeLeafNode node = new BTreeLeafNode(startObjectId, filePosition);
            node.ObjectsCount = objectsCount;

            return node;
        }

        private void ConnectToDataFile ()
        {
            string dataFileName = string.Format(CultureInfo.InvariantCulture, "{0}-{1}-data.dat", storageName, ObjectTypeName);

            dataStream = fileSystem.OpenFileToRead (dataFileName);
            dataReader = new BinaryReader (dataStream);
        }

        private bool disposed;
        private IBTreeNode btree;
        private string storageName;
        private readonly IFileSystem fileSystem;
        private Stream dataStream;
        private BinaryReader dataReader;
    }
}