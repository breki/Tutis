using MbUnit.Framework;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class ConstructorAutowiringTests : ChestTestFixtureBase
    {
        [Test, ExpectedException(typeof(ChestException), "The type's TreasureChest.Tests.SampleModule.ComponentWithNonWiredConstructorParameters candidate constructor has some manually wired arguments, but there are no values.")]
        public void TryToFetchComponentWithManuallyWiredConstructorParametersWithoutValues()
        {
            Chest
                .Add<ComponentWithNonWiredConstructorParameters>();

            using (Lease<ComponentWithNonWiredConstructorParameters> lease 
                = Chest.Fetch<ComponentWithNonWiredConstructorParameters>())
                Assert.IsNotNull(lease.Instance);
        }

        [Test]
        public void FetchComponentWithManuallyWiredConstructorParametersUsingExplicitArguments()
        {
            string value = "something";

            Chest
                .Add<IServiceWithSingleImplementation>()
                .AddCustom.Service<ComponentWithNonWiredConstructorParameters>()
                    .Arg("fileName", value).Done
                .Done();

            using (Lease<ComponentWithNonWiredConstructorParameters> lease
                = Chest.Fetch<ComponentWithNonWiredConstructorParameters>())
            {
                Assert.IsNotNull(lease.Instance);
                Assert.AreEqual(value, lease.Instance.FileName);
            }
        }

        [Test]
        public void ComponentHasMoreThanOneManuallyWiredArgument()
        {
            Chest
                .AddCustom.Service<ComponentWithNonWiredConstructorParameters2>()
                .Arg("arg1", "a").Arg("arg2", "b").Done
                .Done();

            using (var lease = Chest.Fetch<ComponentWithNonWiredConstructorParameters2>())
            {
                Assert.IsNotNull(lease.Instance);
            }
        }

        [Test]
        public void ComponentHasMoreThanOneConstructorButUseShorterOne()
        {
            Chest
                .AddCustom.Service<ComponentWithNonWiredConstructorParametersAndMultipleConstructors>()
                .Arg("arg1", "a").Done
                .Done();

            using (var lease = Chest.Fetch<ComponentWithNonWiredConstructorParametersAndMultipleConstructors>())
            {
                Assert.IsNotNull(lease.Instance);
            }
        }

        [Test]
        public void EnumerableConstructorArg()
        {
            Chest
                .Add<IServiceX, IndependentComponentA>()
                .Add<ComponentWithEnumerableConstructorArg>();

            using (var lease = Chest.Fetch<ComponentWithEnumerableConstructorArg>())
            {
                Assert.IsNotNull(lease.Instance);
                Assert.AreEqual(1, lease.Instance.Services.Count);
            }
        }
    }
}