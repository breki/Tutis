using System;
using System.Collections.Generic;
using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1.RTrees
{
    public class RTreeLeafNode : IRTreeNode
    {
        public Mbr Mbr
        {
            get { return mbr; }
        }

        public bool IsLeaf { get { return true; } }

        public int ObjectsCount
        {
            get { return objects.Count; }
        }

        public IList<RTreeObjectEntry> Objects
        {
            get { return objects; }
        }

        public void AddObject (Tuple<long, Mbr> objectData)
        {
            Mbr objectMbr = objectData.Item2;
            objects.Add(new RTreeObjectEntry(objectData.Item1, objectMbr));
            mbr.ExtendToCover(objectMbr);
        }

        private List<RTreeObjectEntry> objects = new List<RTreeObjectEntry>();
        private Mbr mbr = new Mbr();
    }
}