using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMetaSharp.UnitTests
{
    public class Program : IPerformTests
    {
        public static void Main()
        {
            var p = new Program();
            p.PerformTests();
        }

        public void PerformTests()
        {
            // Test the basic infrastructure lists
            OMetaListTests.PerformTests();
            OMetaFlatListTests.PerformTests();
            OMetaStringListTests.PerformTests();
        }
    }
}
