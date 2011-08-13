using MbUnit.Framework;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class ResolvingTests : ChestTestFixtureBase
    {
        [Test, ExpectedException (typeof(ChestException), "Singleton service implementation TreasureChest.Tests.SampleModule.DependentComponentA instantiation is already in progress, looks like you have a dependency cycle somewhere.")]
        public void PreventCycles()
        {
            Chest
                .Add<IServiceX, DependentComponentA>()
                .Add<IServiceY, ServiceImplYThatDependsOnServiceX>();

            using (var lease = Chest.Fetch<IServiceX>())
            {
            }
        }
    }
}