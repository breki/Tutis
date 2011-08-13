using System.Collections.Generic;
using MbUnit.Framework;
using TreasureChest.Policies;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class DefaultAutowiringConstructorArgumentTypesPolicyTests
    {
        [Test]
        public void Test()
        {
            DefaultAutowiringConstructorArgumentTypesPolicy policy 
                = new DefaultAutowiringConstructorArgumentTypesPolicy();

            Assert.IsFalse(policy.ShouldArgumentTypeBeAutowired("test".GetType()));
            Assert.IsFalse(policy.ShouldArgumentTypeBeAutowired(new string[0].GetType()));
            Assert.IsTrue(policy.ShouldArgumentTypeBeAutowired(typeof(IServiceX)));
            Assert.IsTrue(policy.ShouldArgumentTypeBeAutowired(new List<IServiceX>().GetType()));
        }
    }
}