using MbUnit.Framework;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class ResolvingTests : ChestTestFixtureBase
    {
        [Test, ExpectedException(typeof(ChestException), "Looks like there is a cycle in the dependency tree. Service TreasureChest.Tests.SampleModule.DependentComponentA has been visited for the second time: TreasureChest.Tests.SampleModule.IServiceX -> TreasureChest.Tests.SampleModule.DependentComponentA -> TreasureChest.Tests.SampleModule.IServiceY -> TreasureChest.Tests.SampleModule.ServiceImplYThatDependsOnServiceX -> TreasureChest.Tests.SampleModule.IServiceX -> TreasureChest.Tests.SampleModule.DependentComponentA.")]
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