using MbUnit.Framework;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class TransientsTests : ChestTestFixtureBase
    {
        [Test, ExpectedException(typeof(ChestException), "TransientLifestyle does not allow injecting instances.")]
        public void InsertingTransientInstancesDoesNotMakeSense()
        {
            Chest
                .SetDefaultLifestyle<TransientLifestyle>()
                .AddInstance(new IndependentComponentA());
        }
    }
}