using System;
using System.Collections.Generic;

namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public class BTreeLeafNode : IBTreeNode
    {
        public BTreeLeafNode (long startObjectId, long filePosition)
        {
            this.startObjectId = startObjectId;
            this.filePosition = filePosition;
        }

        public long StartObjectId
        {
            get { return startObjectId; }
        }

        public bool IsLeaf { get { return true; } }

        public long FilePosition
        {
            get { return filePosition; }
        }

        public int ObjectsCount
        {
            get { return objectsCount; }
            set { objectsCount = value; }
        }

        public long? NextBlockStartObjectId
        {
            get
            {
                if (nextBlockStartObjectId == 0)
                    return null;

                return nextBlockStartObjectId;
            }
        }

        public void VisitAllNodes(Action<IBTreeNode> visitAction)
        {
            visitAction(this);
        }

        public BTreeLeafNode FindLeafNode (long objectId)
        {
            return this;
        }

        public void SetNextBlockObjectId (long objectId)
        {
            if (objectId <= startObjectId)
                throw new InvalidOperationException ("objectId <= startObjectId");

            nextBlockStartObjectId = objectId;
        }

        private long startObjectId;
        private long filePosition;
        private int objectsCount;
        private long nextBlockStartObjectId;
    }
}