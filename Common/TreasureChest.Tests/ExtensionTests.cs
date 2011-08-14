using MbUnit.Framework;
using Rhino.Mocks;
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

        //[Test]
        //public void ExtensionShouldBeDisposedByChestDisposeMethod()
        //{
        //    IChestExtension extension = MockRepository.GenerateMock<IChestExtension>();
        //    extension.Expect(x => x.Dispose());

        //    Chest.InstallExtension(extension);

        //    Chest.Dispose();
        //    Chest = null;

        //    extension.VerifyAllExpectations();
        //}
    }
}