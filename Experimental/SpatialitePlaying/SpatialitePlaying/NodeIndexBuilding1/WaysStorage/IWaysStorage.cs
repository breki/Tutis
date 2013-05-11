﻿using Brejc.OsmLibrary;

namespace SpatialitePlaying.NodeIndexBuilding1.WaysStorage
{
    public interface IWaysStorage
    {
        void InitializeForWriting ();
        void WriteWay (OsmWay way);
        void CloseForWriting ();        
    }
}