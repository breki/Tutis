using System.Collections.Generic;
using System.IO;
using Brejc.Common.FileSystem;
using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1.WaysIndexing
{
    public class WaysBTreeIndex : OsmObjectBTreeIndexBase, IWaysBTreeIndex
    {
        public WaysBTreeIndex(IFileSystem fileSystem) : base(fileSystem)
        {
        }

        public IDictionary<long, WayData> FetchWays(IEnumerable<long> ids)
        {
            Dictionary<long, WayData> waysData = new Dictionary<long, WayData> ();

            BTreeLeafNode currentLeafNode = null;
            int? startingIndex = null;
            foreach (long id in ids)
            {
                if (currentLeafNode == null || id >= currentLeafNode.NextBlockStartObjectId)
                {
                    currentLeafNode = Btree.FindLeafNode (id);
                    startingIndex = null;
                }

                WayData nodeData = GetWayData (id, currentLeafNode, ref startingIndex);
                waysData.Add (id, nodeData);
            }

            return waysData;
        }

        protected override string ObjectTypeName
        {
            get { return "ways"; }
        }

        private WayData GetWayData (long id, BTreeLeafNode treeLeaf, ref int? startingIndex)
        {
            WaysDataBlock waysDataBlock;

            long blockId = treeLeaf.FilePosition;
            if (!cachedBlocks.TryGetValue (blockId, out waysDataBlock))
            {
                waysDataBlock = ReadWaysDataBlock (treeLeaf);

                if (cachedBlocks.Count > blocksCacheSize)
                {
                    long blockIdToRemove = nodeBlocksLoadingQueue.Dequeue ();
                    cachedBlocks.Remove (blockIdToRemove);
                }

                cachedBlocks.Add (blockId, waysDataBlock);
                nodeBlocksLoadingQueue.Enqueue (blockId);
            }

            return waysDataBlock.GetWayData (id, ref startingIndex);
        }

        private WaysDataBlock ReadWaysDataBlock (BTreeLeafNode treeLeaf)
        {
            DataReader.BaseStream.Seek (treeLeaf.FilePosition, SeekOrigin.Begin);

            WaysDataBlock block = new WaysDataBlock (treeLeaf.ObjectsCount);

            for (int i = 0; i < treeLeaf.ObjectsCount; i++)
            {
                long id = dataReader.ReadInt64 ();
                Mbr mbr = Mbr.ReadFromStream(dataReader);
                int pointsBlobLen = dataReader.ReadInt32();
                byte[] pointsBlob = dataReader.ReadBytes(pointsBlobLen);
                WayData wayData = new WayData (id, mbr, pointsBlob);

                block.SetWayData (i, wayData);
            }

            return block;
        }

        private Dictionary<long, WaysDataBlock> cachedBlocks = new Dictionary<long, WaysDataBlock> ();
        private Queue<long> nodeBlocksLoadingQueue = new Queue<long> ();
        private int blocksCacheSize = 5000;
    }
}