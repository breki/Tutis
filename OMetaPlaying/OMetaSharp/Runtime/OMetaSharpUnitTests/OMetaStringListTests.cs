using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMetaSharp.UnitTests
{
    public static class OMetaStringListTests
    {
        public static void PerformTests()
        {
            SingleCharTests();
            HelloWorldTests();
            EmptyListTests();
        }
        
        private static void SingleCharTests()
        {
            var l = new OMetaStringList('a');
            Assert.AreEqual(1, l.Count);
            Assert.AreEqual('a', l.Head);
            Assert.AreEqual("a", l);
            Assert.AreEqual('a', l);
            Assert.AreEqual('a', l[0]);
            Assert.IsFalse(l.HasTail);
            Assert.IsTrue(l.IsReadOnly);
            Assert.IsTrue(l.IsSingleItem);
        }

        private static void HelloWorldTests()
        {
            var l = (OMetaStringList)"Hello World!";
            Assert.AreEqual('H', l.Head);
            Assert.AreEqual(12, l.Count);
            Assert.AreEqual('H', l[0]);
            Assert.AreEqual('e', l[1]);
            Assert.AreEqual('l', l[2]);
            Assert.AreEqual('l', l[3]);
            Assert.AreEqual('o', l[4]);
            Assert.AreEqual("Hello World!", l);
            Assert.AreEqual("'Hello World!'", l.ToString());
            Assert.AreEqual("ello World!", l.Tail);
            Assert.AreEqual("llo World!", l.Tail.Tail);
            Assert.IsTrue(l.IsReadOnly);
            Assert.IsTrue(!l.IsSingleItem);
        }

        private static void EmptyListTests()
        {
            var l = new OMetaStringList("");
            Assert.AreEqual("''", l.ToString());
        }
    }
}
