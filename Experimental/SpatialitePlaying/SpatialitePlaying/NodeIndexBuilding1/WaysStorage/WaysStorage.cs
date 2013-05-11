﻿using System;
using System.Collections.Generic;
using System.IO;
using Brejc.Common.BinaryProcessing;
using Brejc.Common.FileSystem;
using Brejc.Geometry;
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

        public void WriteWay(OsmWay way, IPointD2List points)
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
                writer.Flush();
            }

            writer.Write (way.ObjectId);
            byte[] pointsBlob = PointsToBlob2(points, 1000, true);
            int blobLength = pointsBlob.Length;
            writer.Write(blobLength);
            writer.Write(pointsBlob);

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

        public static byte[] PointsToBlob2 (IPointD2List points, int granularity, bool skipLastPoint)
        {
            const double Nanodegree = 0.000000001;
            double resolution = granularity * Nanodegree;

            List<long> coordDiffs = new List<long> ();
            int pointsCount = points.PointsCount + (skipLastPoint ? -1 : 0);

            long lxi = 0;
            long lyi = 0;

            for (int i = 0; i < pointsCount; i++)
            {
                double x;
                double y;
                points.GetPoint (i, out x, out y);

                long xi = (long)Math.Round (x / resolution);
                long yi = (long)Math.Round (y / resolution);

                if (xi == lxi && yi == lyi)
                    continue;

                coordDiffs.Add (xi - lxi);
                coordDiffs.Add (yi - lyi);

                lxi = xi;
                lyi = yi;
            }

            pointsCount = coordDiffs.Count / 2;

            byte[] data;
            using (MemoryStream stream = new MemoryStream ())
            using (BinaryWriter writer = new BinaryWriter (stream))
            {
                writer.WriteVarint ((ulong)pointsCount);

                foreach (long coordDiff in coordDiffs)
                    writer.WriteVarint (coordDiff);

                writer.Flush ();
                data = stream.ToArray ();
            }

            return data;
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