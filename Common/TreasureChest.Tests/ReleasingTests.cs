using MbUnit.Framework;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class ReleasingTests : ChestTestFixtureBase
    {
        [Test]
        public void UseAndReturnIndependentSingleton()
        {
            Chest
                .Add<IndependentComponentA>();

            using (var lease = Chest.Fetch<IndependentComponentA>())
                Assert.AreEqual(1, Chest.ObjectsContainedCount);

            Assert.AreEqual(1, Chest.ObjectsContainedCount);
        }

        [Test]
        public void UseAndReturnIndependentTransient()
        {
            Chest
                .AddTransient<IndependentComponentA>();

            using (var lease = Chest.Fetch<IndependentComponentA>())
                Assert.AreEqual(1, Chest.ObjectsContainedCount);

            Assert.AreEqual(0, Chest.ObjectsContainedCount);
        }

        [Test]
        public void UseAndReturnIndependentDisposableTransient()
        {
            Chest
                .AddTransient<DisposableComponentA>();

            DisposableComponentA instance;
            using (var lease = Chest.Fetch<DisposableComponentA>())
            {
                instance = lease.Instance;
                Assert.AreEqual(1, Chest.ObjectsContainedCount);
            }

            Assert.AreEqual(0, Chest.ObjectsContainedCount);
            Assert.IsTrue(instance.Disposed);
        }

        [Test]
        public void UseAndReturnDependentTransient()
        {
            Chest
                .AddTransient<DependentComponentA>()
                .AddTransient<IServiceY, ServiceImplY>();

            using (var lease = Chest.Fetch<DependentComponentA>())
                Assert.AreEqual(2, Chest.ObjectsContainedCount);

            Assert.AreEqual(0, Chest.ObjectsContainedCount);
        }

        [Test]
        public void InstancesShouldNotBeReleased()
        {
            IndependentComponentA obj = new IndependentComponentA();

            Chest.AddInstance(obj);
            Assert.AreEqual(1, Chest.ObjectsContainedCount);

            using (Lease<IndependentComponentA> lease = Chest.Fetch<IndependentComponentA>())
                Assert.AreEqual(1, Chest.ObjectsContainedCount);

            Assert.AreEqual(1, Chest.ObjectsContainedCount);
        }

        [Test]
        public void BeforeComponentIsDestroyed()
        {
            ComponentWithStaticMember.Value = 10;

            Chest
                .AddCustom.Service<ComponentWithStaticMember>()
                    .WithLifestyle<TransientLifestyle>()
                    .OnDestroyDo(x => ComponentWithStaticMember.Value++)
                    .Done
                .Done();

            using (var lease = Chest.Fetch<ComponentWithStaticMember>())
            {
            }

            Assert.AreEqual(11, ComponentWithStaticMember.Value);
        }

        [Test]
        public void DisposingOfChestShouldReleaseEverything()
        {
            DestroyedComponentTrackingPolicy policy = new DestroyedComponentTrackingPolicy();
            Chest.SetPolicy(policy);

            DisposableComponentA obj = new DisposableComponentA();

            Chest.AddInstance(obj);
            Assert.AreEqual(1, Chest.ObjectsContainedCount);

            Chest.Dispose();

            Assert.IsTrue(obj.Disposed);

            Assert.AreEqual(1, policy.Counter);
        }

        [Test]
        public void ManuallyRegisteringDependencyShouldAffectOrderOfDisposingComponents1()
        {
            DestroyedComponentTrackingPolicy policy = new DestroyedComponentTrackingPolicy ();
            Chest.SetPolicy (policy);

            DisposableComponentA obj1 = new DisposableComponentA ();
            IndependentComponentA obj2 = new IndependentComponentA();

            Chest.AddInstance (obj1);
            Chest.AddInstance (obj2);
            Chest.RegisterDependency (obj2, obj1);

            Chest.Dispose ();

            Assert.AreEqual (2, policy.Counter);
            Assert.AreEqual ("TreasureChest.Tests.SampleModule.DisposableComponentA", policy.DestroyedComponents[0]);
            Assert.AreEqual ("TreasureChest.Tests.SampleModule.IndependentComponentA", policy.DestroyedComponents[1]);
        }

        [Test]
        public void ManuallyRegisteringDependencyShouldAffectOrderOfDisposingComponents2 ()
        {
            DestroyedComponentTrackingPolicy policy = new DestroyedComponentTrackingPolicy ();
            Chest.SetPolicy (policy);

            DisposableComponentA obj1 = new DisposableComponentA ();
            IndependentComponentA obj2 = new IndependentComponentA ();

            Chest.AddInstance (obj1);
            Chest.AddInstance (obj2);
            Chest.RegisterDependency (obj1, obj2);

            Chest.Dispose ();

            Assert.AreEqual (2, policy.Counter);
            Assert.AreEqual ("TreasureChest.Tests.SampleModule.IndependentComponentA", policy.DestroyedComponents[0]);
            Assert.AreEqual ("TreasureChest.Tests.SampleModule.DisposableComponentA", policy.DestroyedComponents[1]);
        }
    }
}