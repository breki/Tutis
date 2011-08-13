using MbUnit.Framework;

namespace TreasureChest.Tests
{
    public class ChestTestFixtureBase
    {
        protected Chest Chest
        {
            get { return chest; }
        }

        [SetUp]
        protected virtual void Setup()
        {
            chest = new Chest();
        }

        [TearDown]
        protected virtual void Teardown()
        {
            if (chest != null)
                chest.Dispose();
        }

        private Chest chest;
    }
}