using System;
using System.Collections;
using System.Collections.Generic;
using MbUnit.Framework;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class ReflectionTests
    {
        [Test]
        public void TestIsGenericEnumerable()
        {
            Type innerType;

            IEnumerable<IServiceX> x = new List<IServiceX>();
            Type type = x.GetType();
            Assert.IsTrue(type.IsGenericEnumerable(out innerType));
            Assert.AreEqual(typeof(IServiceX), innerType);

            IEnumerable y = new Hashtable();
            type = y.GetType();
            Assert.IsFalse(type.IsGenericEnumerable(out innerType));
            Assert.IsNull(innerType);
        }
    }
}