using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Brejc.Common.FileSystem;

namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public abstract class IndexedOsmObjectStorageWriterBase : IIndexedOsmObjectStorageWriter
    {
        public void InitializeStorage (string storageName)
        {
            stream = fileSystem.OpenFileToWrite (string.Format(CultureInfo.InvariantCulture, "{0}-{1}-data.dat", storageName, ObjectTypeName));
            writer = new BinaryWriter (stream);
        }

        public void FinalizeStorage()
        {
            if (previousLeafNode != null)
                previousLeafNode.ObjectsCount = itemsInBlockCounter;

            writer.Close ();
            writer.Dispose ();
            writer = null;
            stream.Close ();
            stream.Dispose ();
            stream = null;

            ConstructBTree ();
        }

        protected abstract string ObjectTypeName { get; }

        protected BinaryWriter Writer
        {
            get { return writer; }
        }

        protected IndexedOsmObjectStorageWriterBase (IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        protected void FlushCurrentBlockIfFull(long nextObjectId)
        {
            if (itemsInBlockCounter%100 != 0) 
                return;

            if (previousLeafNode != null)
                previousLeafNode.SetNextBlockObjectId (nextObjectId);

            if (previousLeafNode != null)
                previousLeafNode.ObjectsCount = itemsInBlockCounter;

            BTreeLeafNode leafNode = new BTreeLeafNode (nextObjectId, stream.Position);
            leafNodes.Add (leafNode);
            itemsInBlockCounter = 1;
            previousLeafNode = leafNode;
            Writer.Flush ();
        }

        private void ConstructBTree ()
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
                        BTreeNonLeafNode parentNode = new BTreeNonLeafNode (
                            node1, node2);
                        upperLevel.Add (parentNode);
                    }
                }

                lowerLevel.Clear ();
                lowerLevel = upperLevel;
                upperLevel = new List<IBTreeNode> ();
            }

            btree = lowerLevel[0];
            leafNodes.Clear ();
        }

        private readonly IFileSystem fileSystem;
        private Stream stream;
        private BinaryWriter writer;
        private int itemsInBlockCounter;
        private List<BTreeLeafNode> leafNodes = new List<BTreeLeafNode> ();
        private IBTreeNode btree;
        private BTreeLeafNode previousLeafNode;
    }
}