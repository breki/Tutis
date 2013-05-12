using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Brejc.Common.FileSystem;
using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1.RTrees
{
    public class NaiveRTreeConstructor : IRTreeConstructor
    {
        public NaiveRTreeConstructor(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public void InitializeStorage(string storageName, string objectTypeName)
        {
            this.storageName = storageName;
            this.objectTypeName = objectTypeName;
        }

        public void AddObject(long objectId, Mbr objectMbr)
        {
            objects.Add(new Tuple<long, Mbr>(objectId, objectMbr));
        }

        public void ConstructRTree()
        {
            SortObjects();
            IRTreeNode rtree = ConstructRTreePrivate();
            SaveRTree(rtree);
        }

        private void SortObjects()
        {
            objects.Sort((a, b) => a.Item2.MinX.CompareTo(b.Item2.MinX));
        }

        private IRTreeNode ConstructRTreePrivate()
        {
            List<IRTreeNode> upperLevel = new List<IRTreeNode> ();
            List<IRTreeNode> lowerLevel = ConstructLeafNodes();

            while (lowerLevel.Count > 1)
            {
                RTreeInnerNode currentNode = null;
                for (int i = 0; i < lowerLevel.Count; i++)
                {
                    IRTreeNode treeNode = lowerLevel[i];

                    if (currentNode == null || currentNode.ChildrenCount >= leafNodeCapacity)
                    {
                        currentNode = new RTreeInnerNode ();
                        upperLevel.Add (currentNode);
                    }

                    currentNode.AddChildNode(treeNode);
                }

                lowerLevel.Clear ();
                lowerLevel = upperLevel;
                upperLevel = new List<IRTreeNode> ();
            }

            IRTreeNode rtree = lowerLevel[0];

            return rtree;
        }

        private List<IRTreeNode> ConstructLeafNodes()
        {
            List<IRTreeNode> leafNodes = new List<IRTreeNode>();

            RTreeLeafNode currentNode = null;
            for (int i = 0; i < objects.Count; i++)
            {
                Tuple<long, Mbr> obj = objects[i];
                if (currentNode == null || currentNode.ObjectsCount >= leafNodeCapacity)
                {
                    currentNode = new RTreeLeafNode();
                    leafNodes.Add(currentNode);
                }

                currentNode.AddObject(obj);
            }

            return leafNodes;
        }

        private void SaveRTree(IRTreeNode rtree)
        {
            string dataFileName = string.Format (CultureInfo.InvariantCulture, "{0}-{1}-rtree.idx", storageName, objectTypeName);

            using (Stream stream = fileSystem.OpenFileToWrite(dataFileName))
            using (BinaryWriter writer = new BinaryWriter(stream))
                WriteRTreeNode(rtree, true, writer);
        }

        private static void WriteRTreeNode(IRTreeNode node, bool isRoot, BinaryWriter writer)
        {
            if (isRoot)
                node.Mbr.WriteToStream (writer);

            if (!node.IsLeaf)
                WriteRTreeInnerNode((RTreeInnerNode)node, writer);
            else
                WriteRTreeLeafNode ((RTreeLeafNode)node, writer);
        }

        private static void WriteRTreeInnerNode(RTreeInnerNode node, BinaryWriter writer)
        {
            writer.Write((byte)0);
            writer.Write(node.ChildrenCount);

            foreach (IRTreeNode child in node.Children)
                child.Mbr.WriteToStream(writer);

            foreach (IRTreeNode child in node.Children)
                WriteRTreeNode(child, false, writer);
        }

        private static void WriteRTreeLeafNode(RTreeLeafNode node, BinaryWriter writer)
        {
            writer.Write ((byte)1);
            writer.Write (node.ObjectsCount);

            foreach (RTreeObjectEntry obj in node.Objects)
                obj.Mbr.WriteToStream (writer);

            foreach (RTreeObjectEntry obj in node.Objects)
                writer.Write(obj.ObjectId);
        }

        private string storageName;
        private List<Tuple<long, Mbr>> objects = new List<Tuple<long, Mbr>>();
        private int leafNodeCapacity = 100;
        private readonly IFileSystem fileSystem;
        private string objectTypeName;
    }
}