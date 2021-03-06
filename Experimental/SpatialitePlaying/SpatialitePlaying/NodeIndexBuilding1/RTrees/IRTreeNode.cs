﻿using System.Collections.Generic;
using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1.RTrees
{
    public interface IRTreeNode
    {
        Mbr Mbr { get; }
        bool IsLeaf { get; }
        void FindObjects(Mbr insideMbr, IList<long> ids);
    }
}