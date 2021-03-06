﻿using System.Collections.Generic;
using SpatialitePlaying.NodeIndexBuilding1.OsmObjectIndexing;

namespace SpatialitePlaying.NodeIndexBuilding1.RTrees
{
    public interface ISpatialQuery
    {
        void Connect(string storageName, string objectTypeName);
        List<long> FindObjects(Mbr insideMbr);
    }
}