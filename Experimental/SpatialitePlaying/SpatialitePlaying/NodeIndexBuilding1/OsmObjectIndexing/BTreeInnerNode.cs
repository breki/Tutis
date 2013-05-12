using System;
using System.Collections.Generic;

namespace SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing
{
    public class BTreeInnerNode : IBTreeNode
    {
        public BTreeInnerNode (IBTreeNode leftNode, IBTreeNode rightNode)
        {
            this.leftNode = leftNode;
            this.rightNode = rightNode;
            leftNodeStartId = leftNode.StartObjectId;
            rightNodeStartId = rightNode.StartObjectId;

#if DEBUG
            if (leftNodeStartId >= rightNodeStartId)
                throw new InvalidOperationException ("leftNodeStartId >= rightNodeStartId");
#endif
        }

        public long StartObjectId { get { return leftNodeStartId; } }
        public bool IsLeaf { get { return false; } }

        public long LeftNodeStartId
        {
            get { return leftNodeStartId; }
        }

        public long RightNodeStartId
        {
            get { return rightNodeStartId; }
        }

        public IBTreeNode LeftNode
        {
            get { return leftNode; }
        }

        public IBTreeNode RightNode
        {
            get { return rightNode; }
        }

        public void VisitAllNodes(Action<IBTreeNode> visitAction)
        {
            visitAction(this);
            leftNode.VisitAllNodes(visitAction);
            rightNode.VisitAllNodes(visitAction);
        }

        public BTreeLeafNode FindLeafNode (long objectId)
        {
            if (objectId < rightNodeStartId)
                return leftNode.FindLeafNode(objectId);

            return rightNode.FindLeafNode(objectId);
        }

        private IBTreeNode leftNode;
        private IBTreeNode rightNode;
        private readonly long leftNodeStartId;
        private readonly long rightNodeStartId;
    }
}