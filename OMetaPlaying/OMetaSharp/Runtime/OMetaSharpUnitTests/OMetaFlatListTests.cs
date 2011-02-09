using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMetaSharp.UnitTests
{
    public static class OMetaFlatListTests
    {
        public static void PerformTests()
        {
            EmptyListTests();
            SingleItemTests();
            TwoItemTests();
            ThreeItemTests();
        }

        private static void EmptyListTests()
        {
            var empty = new OMetaFlatList<int>();
            Assert.AreEqual("[]", empty);
            Assert.AreEqual(0, empty.Count);
        }

        private static void SingleItemTests()
        {
            OMetaListTests.SingleItemTests(new OMetaFlatList<int>(1));
        }

        private static void TwoItemTests()
        {
            OMetaListTests.TwoItemTests(new OMetaFlatList<int>(1, 2));
        }

        private static void ThreeItemTests()
        {
            OMetaListTests.ThreeItemTests(new OMetaFlatList<int>(1, 2, 3));
        }
    }
}
