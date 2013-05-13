using System;
using SpatialitePlaying.NodeIndexBuilding1;

namespace SpatialitePlaying
{
    class Program
    {
        static void Main (string[] args)
        {
            RTreeTests tests = new RTreeTests();
            tests.RunQuery();
        }
    }
}
