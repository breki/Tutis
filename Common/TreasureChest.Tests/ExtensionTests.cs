using MbUnit.Framework;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class ExtensionTests : ChestTestFixtureBase
    {
        [Test, ExpectedException(typeof(ChestException), "The required extension TreasureChest.Tests.SampleModule.SampleExtension is not installed.")]
        public void ThrowExceptionIfExtensionIsNotInstalled()
        {
            Chest.AssertExtensionIsInstalled<SampleExtension>();
        }

        [Test]
        public void AssertIsInstalled()
        {
            Chest
                .InstallExtension<SampleExtension>()
                .AssertExtensionIsInstalled<SampleExtension>();            
        }
    }
}