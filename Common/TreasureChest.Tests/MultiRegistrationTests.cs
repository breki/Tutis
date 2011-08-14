using System.Collections.Generic;
using System.Linq;
using MbUnit.Framework;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class MultiRegistrationTests : ChestTestFixtureBase
    {
        [Test, ExpectedException(typeof(ChestException), "Service TreasureChest.Tests.SampleModule.IServiceY is not registered")]
        public void TryToFetchAllWhenSomeServicesAreNotRegistered()
        {
            //Chest
            //    .FindPolicy<DefaultConstructionPolicy>()
            //        .For<ComponentWithMultipleConstructors>().UseConstructorWithParameters()

            Chest
                .AddCustom.AllImplementationsOf<IServiceX>().Done
                .Done();

            Assert.IsTrue(Chest.HasService<IServiceX>());
            IEnumerable<IServiceX> objects = Chest.FetchAll<IServiceX>();
            Assert.AreEqual(5, objects.Count());
        }

        [Test]
        public void FetchAll()
        {
            Chest
                .AddCustom.AllImplementationsOf<IServiceX>().Done
                .Add<IServiceY, ServiceImplY>()
                .Add<IServiceWithSingleImplementation>()
                .Done();

            Assert.IsTrue(Chest.HasService<IServiceX>());
            IEnumerable<IServiceX> objects = Chest.FetchAll<IServiceX>();
            Assert.AreEqual(6, objects.Count());
        }

        [Test]
        public void ImplementationShouldBeRegisteredAsTheirOwnToo()
        {
            Chest
                .AddCustom.AllImplementationsOf<IServiceX>().Done
                .Done();

            Assert.IsTrue(Chest.HasService<IndependentComponentA>());
            using (var lease = Chest.Fetch<IndependentComponentA>())
            {
            }
        }

        [Test]
        public void ForEachClassTest()
        {
            Chest.RegisterAssemblyOf<IServiceX>();
            Chest
                .Add<IServiceY, ServiceImplY>()
                .Add<IServiceWithSingleImplementation>()
                .AddCustom.ForEachClass(t => typeof(IServiceX).IsAssignableFrom(t))
                    .Do((c, t) => c.Add(typeof(IServiceX), t));

            IEnumerable<IServiceX> objects = Chest.FetchAll<IServiceX>();
            Assert.AreEqual(6, objects.Count());
        }
    }
}