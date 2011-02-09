using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMetaSharp.UnitTests
{
    public static class OMetaListTests
    {
        public static void PerformTests()
        {
            NilTests();
            SingleItemTests();
            TwoItemTests();
            ThreeItemTests();
            TreeTests();
            ToStringTests();
        }
        
        private static void NilTests()
        {
            // Might want to make these nicer given the host language lookup?
            Assert.AreEqual("nil<Int32>", OMetaList<int>.Nil);

            // TODO: Generics would look nicer as nil<List<int>>, 
            // I don't know of an cheap way of doing this.
            Assert.AreEqual("nil<List`1>", OMetaList<List<int>>.Nil);
            Assert.AreEqual("nil<List`1>", OMetaList<List<List<int>>>.Nil);
        }

        private static void SingleItemTests()
        {
            SingleItemTests((OMetaList<int>)1);
        }

        internal static void SingleItemTests(OMetaList<int> singleItem)
        {
            Assert.AreEqual(1, (int)singleItem);
            Assert.AreEqual(1, (int)singleItem[0]);
            Assert.AreEqual(1, (int)singleItem[0][0][0][0]);
            Assert.AreEqual(1, singleItem);
            Assert.AreEqual(1, (int)singleItem.Head);
            Assert.AreEqual(1, singleItem.Count);
            Assert.AreEqual("1", singleItem);
            Assert.IsTrue(singleItem.IsReadOnly);
            Assert.IsTrue(singleItem.IsSingleItem);
            Assert.IsFalse(singleItem.HasTail);
            Assert.AreEqual(OMetaList<int>.Nil, singleItem.Tail);
        }

        private static void TwoItemTests()
        {
            TwoItemTests(OMetaList<int>.Concat(1, 2));
        }

        internal static void TwoItemTests(OMetaList<int> twoItems)
        {
            Assert.AreEqual(2, twoItems.Count);
            Assert.AreEqual(1, (int)twoItems.Head);
            Assert.AreEqual(1, (int)twoItems[0]);
            Assert.AreEqual(2, (int)twoItems[1]);
            Assert.IsTrue(twoItems.HasTail);
            Assert.AreEqual(2, (int)twoItems.Tail);
            Assert.AreEqual("[1, 2]", twoItems);
            Assert.AreEqual("1", twoItems.Head);
            Assert.AreEqual("2", twoItems.Tail);
        }

        private static void ThreeItemTests()
        {
            ThreeItemTests(OMetaList<int>.Concat(1, 2, 3));
        }

        internal static void ThreeItemTests(OMetaList<int> items)
        {
            Assert.AreEqual("[1, 2, 3]", items);
            Assert.AreEqual(1, items[0]);
            Assert.AreEqual(2, items[1]);
            Assert.AreEqual(3, items[2]);
            Assert.AreEqual("[2, 3]", items.Tail);
        }

        private static void TreeTests()
        {
            var t = new OMetaList<int>(OMetaList<int>.Concat(1, 2, 3), OMetaList<int>.Concat(4, 5, 6));
            Assert.AreEqual(4, t.Count);
            Assert.AreEqual("[[1, 2, 3], 4, 5, 6]", t);

            t = new OMetaList<int>(OMetaList<int>.Concat(1, 2, 3),
                                       new OMetaList<int>(new OMetaList<int>(4, new OMetaList<int>(OMetaList<int>.Concat(5, 6, 7))),
                                                          OMetaList<int>.Concat(8, 9)));
            Assert.AreEqual(4, t.Count);
            Assert.AreEqual("[[1, 2, 3], [4, [5, 6, 7]], 8, 9]", t);

            t = OMetaList<int>.ConcatLists(
                    OMetaList<int>.Concat(1, 2, 3),
                    OMetaList<int>.Concat(4, 5, 6),
                    OMetaList<int>.ConcatLists(
                        new OMetaList<int>(7),
                        OMetaList<int>.Concat(8, 9, 10)));

            Assert.AreEqual("[[1, 2, 3], [4, 5, 6], [7, [8, 9, 10]]]", t);
        }

        private static void ToStringTests()
        {
            var t = OMetaList<HostExpression>.ConcatLists("And".AsHostExpressionList(),
                OMetaList<HostExpression>.ConcatLists(
                    "App".AsHostExpressionList(),
                    "Exactly".AsHostExpressionList(),
                    "123".AsHostExpressionList()));

            Assert.AreEqual("[And, [App, Exactly, 123]]", t.ToString());

        }
    }
}
