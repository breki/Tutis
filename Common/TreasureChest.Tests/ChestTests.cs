using MbUnit.Framework;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class ChestTests : ChestTestFixtureBase
    {
        [Test, ExpectedException(typeof(ChestException), "Service TreasureChest.Tests.SampleModule.IndependentComponentA is not registered (TreasureChest.Tests.SampleModule.IndependentComponentA).")]
        public void TryToFetchSomethingThatIsNotThere()
        {
            Chest.Fetch<IndependentComponentA>();            
        }

        [Test]
        public void FetchIndependentComponent()
        {
            Chest.Add<IndependentComponentA>();
            using (Lease<IndependentComponentA> lease = Chest.Fetch<IndependentComponentA>())
                Assert.IsNotNull(lease.Instance);
        }

        [Test]
        public void RegisterComponentImplementingAService()
        {
            Chest.Add<IServiceX, IndependentComponentA>();
            using (Lease<IServiceX> lease = Chest.Fetch<IServiceX>())
                Assert.IsNotNull(lease.Instance);
        }

        [Test]
        public void FetchingSingletonComponentTwiceShouldResultInTheSameInstance()
        {
            Chest.Add<IndependentComponentA>();
            IndependentComponentA obj1 = Chest.Fetch<IndependentComponentA>();
            IndependentComponentA obj2 = Chest.Fetch<IndependentComponentA>();
            Assert.AreSame(obj1, obj2);
        }

        [Test]
        public void FetchingTransientComponentTwiceShouldResultInDifferentInstances()
        {
            Chest.AddTransient<IndependentComponentA>();
            IndependentComponentA obj1 = Chest.Fetch<IndependentComponentA>();
            IndependentComponentA obj2 = Chest.Fetch<IndependentComponentA>();
            Assert.AreNotSame(obj1, obj2);
        }

        [Test, ExpectedException(typeof(ChestException),
            "You can not register components with different lifestyles for the same service (service=TreasureChest.Tests.SampleModule.IServiceX, impl1=TreasureChest.Tests.SampleModule.IndependentComponentA, impl2=TreasureChest.Tests.SampleModule.IndependentComponentB).")]
        public void TryToRegisterSameServiceWithDifferentLifestyles()
        {
            Chest
                .SetDefaultLifestyle<ThreadSingletonLifestyle>()
                .Add<IServiceX, IndependentComponentA>()
                .AddTransient<IServiceX, IndependentComponentB>();
        }

        [Test, ExpectedException(typeof(ChestException), "Service TreasureChest.Tests.SampleModule.IndependentComponentA + implementation TreasureChest.Tests.SampleModule.IServiceX has already been registered.")]
        public void TryToRegisterSameServiceAndComponentTwice()
        {
            Chest
                .Add<IServiceX, IndependentComponentA>()
                .Add<IServiceX, IndependentComponentA>();
        }

        [Test, ExpectedException(typeof(ChestException), "Service TreasureChest.Tests.SampleModule.IServiceY is not registered (TreasureChest.Tests.SampleModule.DependentComponentA -> TreasureChest.Tests.SampleModule.IServiceY).")]
        public void TryToFetchDependentComponentWhenDependencyIsNotRegistered()
        {
            Chest
                .Add<DependentComponentA>()
                .Fetch<DependentComponentA>();            
        }

        [Test]
        public void FetchDependentComponent()
        {
            Chest
                .Add<DependentComponentA>()
                .Add<IServiceY, ServiceImplY>();

            using (Lease<DependentComponentA> lease = Chest.Fetch<DependentComponentA>())
                Assert.IsNotNull(lease.Instance.ServiceY);
        }

        [Test]
        public void RegisterSingletonInstance()
        {
            IndependentComponentA obj = new IndependentComponentA();

            Chest.AddInstance(obj);

            using (Lease<IndependentComponentA> lease = Chest.Fetch<IndependentComponentA>())
                Assert.AreSame(obj, lease.Instance);
        }

        [Test]
        public void RegisterComponentWithMultipleServices()
        {
            Chest.Add<IServiceX, IServiceY, ComponentWithMultipleServices>();
            IServiceX obj1 = Chest.Fetch<IServiceX>().Instance;
            IServiceY obj2 = Chest.Fetch<IServiceY>().Instance;
            Assert.AreSame<object>(obj1, obj2);
        }

        [Test]
        public void RegisteringServiceInterfaceWillFindImplementationAutomatically()
        {
            Chest.Add<IServiceWithSingleImplementation>();
            using (Lease<IServiceWithSingleImplementation> lease = Chest.Fetch<IServiceWithSingleImplementation>())
                Assert.IsNotNull(lease.Instance);
        }

        [Test, ExpectedException(typeof(ChestException), "There is more than on implementation for the service TreasureChest.Tests.SampleModule.IServiceX, you have to specify the implementation explicitly.")]
        public void TryToRegisterServiceInterfaceWithMultipleImplementations()
        {
            Chest.Add<IServiceX>();
            using (Lease<IServiceX> lease = Chest.Fetch<IServiceX>())
                Assert.IsNotNull(lease.Instance);
        }

        [Test]
        public void CreateUsingFactoryMethod()
        {
            Chest.AddCustom
                .Service<IServiceX>().FactoryMethod(c => new DependentComponentA(new ServiceImplY())).Done
                .Done();

            using (Lease<IServiceX> lease = Chest.Fetch<IServiceX>())
                Assert.IsNotNull(lease.Instance);
        }
    }
}