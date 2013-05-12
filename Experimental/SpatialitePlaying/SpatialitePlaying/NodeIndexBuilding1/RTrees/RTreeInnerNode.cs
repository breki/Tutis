using System.Collections.Generic;
using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1.RTrees
{
    public class RTreeInnerNode : IRTreeNode
    {
        public Mbr Mbr { get { return mbr; } }

        public bool IsLeaf { get { return false; } }

        public int ChildrenCount { get { return children.Count; } }

        public IList<IRTreeNode> Children
        {
            get { return children; }
        }

        public void AddChildNode (IRTreeNode child)
        {
            children.Add(child);
            mbr.ExtendToCover (child.Mbr);
        }

        private List<IRTreeNode> children = new List<IRTreeNode> ();
        private Mbr mbr = new Mbr();
    }
}