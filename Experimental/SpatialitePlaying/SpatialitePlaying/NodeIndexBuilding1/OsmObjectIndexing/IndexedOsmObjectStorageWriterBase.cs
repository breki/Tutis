using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Brejc.Common.FileSystem;

namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public abstract class IndexedOsmObjectStorageWriterBase : IIndexedOsmObjectStorageWriter
    {
        public virtual void InitializeStorage (string storageName)
        {
            this.storageName = storageName;

            string dataFileName = string.Format(CultureInfo.InvariantCulture, "{0}-{1}-data.dat", storageName, ObjectTypeName);
            stream = fileSystem.OpenFileToWrite (dataFileName);
            writer = new BinaryWriter (stream);
        }

        public virtual void FinalizeStorage()
        {
            if (previousLeafNode != null)
                previousLeafNode.ObjectsCount = itemsInBlockCounter;

            writer.Close ();
            writer.Dispose ();
            writer = null;
            stream.Close ();
            stream.Dispose ();
            stream = null;

            SaveBTree ();
        }

        protected abstract string ObjectTypeName { get; }

        protected BinaryWriter Writer
        {
            get { return writer; }
        }

        public IFileSystem FileSystem
        {
            get { return fileSystem; }
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
            previousLeafNode = leafNode;
            Writer.Flush ();

            itemsInBlockCounter = 0;
        }

        protected void IncrementObjectsInBlockCount ()
        {
            itemsInBlockCounter++;
        }

        private void SaveBTree()
        {
            string dataFileName = string.Format (CultureInfo.InvariantCulture, "{0}-{1}-id.idx", storageName, ObjectTypeName);

            using (Stream btreeStream = fileSystem.OpenFileToWrite(dataFileName))
            using (BinaryWriter btreeWriter = new BinaryWriter(btreeStream))
            {
                foreach (BTreeLeafNode leafNode in leafNodes)
                    WriteTreeNode(leafNode, btreeWriter);

                // write end marker
                btreeWriter.Write(0L);
            }
        }

        private static void WriteTreeNode(IBTreeNode treeNode, BinaryWriter btreeWriter)
        {
            if (treeNode.IsLeaf)
            {
                BTreeLeafNode leafNode = (BTreeLeafNode)treeNode;
                btreeWriter.Write (leafNode.StartObjectId);
                btreeWriter.Write (leafNode.FilePosition);
                btreeWriter.Write (leafNode.ObjectsCount);
            }
        }

        private readonly IFileSystem fileSystem;
        private Stream stream;
        private BinaryWriter writer;
        private int itemsInBlockCounter;
        private List<BTreeLeafNode> leafNodes = new List<BTreeLeafNode> ();
        private BTreeLeafNode previousLeafNode;
        private string storageName;
    }
}