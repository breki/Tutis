using System;
using System.Collections.Generic;
using System.Linq;

namespace WeAreCollections
{
    public static class TestHelper
    {
        public static int[] GenerateRandomUniqueTestSet(Random rnd, int count)
        {
            IEnumerable<int> orderedValues = Enumerable.Range (0, count);
            return orderedValues.OrderBy (a => rnd.Next ()).ToArray ();            
        }
    }
}