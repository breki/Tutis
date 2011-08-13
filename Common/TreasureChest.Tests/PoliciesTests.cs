using MbUnit.Framework;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class PoliciesTests : ChestTestFixtureBase
    {
        [Pending("This will work when we implement Validate() method on chest.")]
        [Test, ExpectedException(typeof(ChestException), "sdsd")]
        public void SingletonsShouldNotDependOnTransients()
        {
            Chest
                .Add<DependentComponentA>()
                .AddTransient<IServiceY, ServiceImplY>();
        }
    }
}