using System;
using MbUnit.Framework;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class CustomRegistrationTests : ChestTestFixtureBase
    {
        [Test]
        public void ImplementedByWithMultipleImplementationsShouldWork()
        {
            Type implType = typeof(IndependentComponentA);

            Chest
                .AddCustom.Service<IServiceX>().ImplementedBy(implType).Done
                .Done();
        }

        [Test]
        public void OnCreateDo()
        {
            Chest
                .AddCustom.Service<ComponentWithNonWiredProperty>()
                    .OnCreateDo(x => x.Value = 20).Done
                .Done();

            using (var lease = Chest.Fetch<ComponentWithNonWiredProperty>())
            {
                Assert.AreEqual(20, lease.Instance.Value);
            }
        }

        [Test]
        public void OnRegisterDo()
        {
            Type implType = null;

            Chest
                .AddCustom.Service<IServiceWithSingleImplementation>()
                    .OnRegisterDo(x => implType = x.ImplType).Done
                .Done();

            Assert.AreEqual(typeof(SingleImplementationOfService), implType);
        }
    }
}