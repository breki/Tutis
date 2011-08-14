using MbUnit.Framework;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class ManualDependenciesRegistrationTests : ChestTestFixtureBase
    {
        [Test]
        public void UsingAfterCreationPolicy()
        {
            Chest
                .Add<IndependentComponentA>()
                .SetPolicy<SampleManualDependencyRegistrationPolicy>()
                .Add<IndependentComponentB>();

            Chest.Fetch<IndependentComponentB>();
        }
    }
}