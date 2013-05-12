using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Brejc.Common.FileSystem;
using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1.RTrees
{
    public class SpatialQuery : ISpatialQuery
    {
        public SpatialQuery(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public void Connect(string storageName, string objectTypeName)
        {
            string dataFileName = string.Format (CultureInfo.InvariantCulture, "{0}-{1}-rtree.idx", storageName, objectTypeName);

            using (Stream stream = fileSystem.OpenFileToRead(dataFileName))
            using (BinaryReader reader = new BinaryReader (stream))
                ReadRTree (reader);
        }

        public IList<long> FindObjects(Mbr insideMbr)
        {
            List<long> ids = new List<long>();

            rTree.FindObjects(insideMbr, ids);

            return ids;
        }

        private void ReadRTree(BinaryReader reader)
        {
            Mbr mbr = Mbr.ReadFromStream(reader);

            rTree = ReadRTreeNode(reader);
        }

        private static IRTreeNode ReadRTreeNode(BinaryReader reader)
        {
            byte nodeTypeIndicator = reader.ReadByte ();
            if (nodeTypeIndicator == 0)
            {
                RTreeInnerNode innerNode = new RTreeInnerNode ();

                int childrenCount = reader.ReadInt32 ();
                List<Mbr> mbrs = new List<Mbr> ();
                for (int i = 0; i < childrenCount; i++)
                    mbrs.Add (Mbr.ReadFromStream (reader));

                for (int i = 0; i < childrenCount; i++)
                {
                    IRTreeNode child = ReadRTreeNode (reader);
                    innerNode.AddChildNode (child);
                }

                return innerNode;
            }

            if (nodeTypeIndicator == 1)
            {
                RTreeLeafNode leafNode = new RTreeLeafNode();

                int objectsCount = reader.ReadInt32();
                List<Mbr> mbrs = new List<Mbr>();
                for (int i = 0; i < objectsCount; i++)
                    mbrs.Add(Mbr.ReadFromStream(reader));

                for (int i = 0; i < objectsCount; i++)
                {
                    long objectId = reader.ReadInt64();
                    leafNode.AddObject(new Tuple<long, Mbr>(objectId, mbrs[i]));
                }

                return leafNode;
            }

            throw new InvalidOperationException("BUG");
        }

        private readonly IFileSystem fileSystem;
        private IRTreeNode rTree;
    }
}