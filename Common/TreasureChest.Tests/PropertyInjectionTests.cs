using MbUnit.Framework;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class PropertyInjectionTests : ChestTestFixtureBase
    {
        [Test]
        public void InjectProperty()
        {
            Chest
                .Add<ComponentWithProperties>()
                .Add<IServiceY, ServiceImplY>();

            using (var lease = Chest.Fetch<ComponentWithProperties>())
            {
                Assert.IsInstanceOfType<ServiceImplY>(lease.Instance.ServiceY);
            }
        }

        [Test]
        public void LeavePropertyUninjectedIfServiceDoesNotExist()
        {
            Chest
                .Add<ComponentWithProperties>();

            using (var lease = Chest.Fetch<ComponentWithProperties>())
            {
                Assert.IsNull(lease.Instance.ServiceY);
            }
        }

        [Test]
        public void LeavePropertyUninjectedIfServiceIsNotSingleton()
        {
            Chest
                .Add<ComponentWithProperties>()
                .AddTransient<IServiceY, ServiceImplY>();

            using (var lease = Chest.Fetch<ComponentWithProperties>())
            {
                Assert.IsNull(lease.Instance.ServiceY);
            }
        }
    }
}