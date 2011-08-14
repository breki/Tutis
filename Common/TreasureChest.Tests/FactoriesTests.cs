using System;
using MbUnit.Framework;
using TreasureChest.Factories;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class FactoriesTests : ChestTestFixtureBase
    {
        [Test]
        public void FactoryProxyShouldImplementBasicObjectMethods()
        {
            FactoryProxy proxy = new FactoryProxy(Chest, typeof(ISampleFactory));
            object transparentProxy = proxy.GetTransparentProxy();
            transparentProxy.GetHashCode();
        }

        [Test]
        public void JustFetchTheFactoryAndReleaseIt()
        {
            Chest
                .AddTransient<IServiceX, IndependentComponentA>()
                .AddFactory<ISampleFactory>();

            using (var lease = Chest.Fetch<ISampleFactory>())
            {
                //IServiceX component = lease.Instance.CreateX();
                //Assert.IsNotNull(component);
            }
        }

        [Test]
        public void CreateBasedOnFactoryMethodReturnType()
        {
            Chest
                .AddTransient<IServiceX, IndependentComponentA>()
                .AddFactory<ISampleFactory>();

            using (var lease = Chest.Fetch<ISampleFactory>())
            {
                IServiceX component = lease.Instance.CreateX();
                Assert.IsNotNull(component);
                Assert.IsInstanceOfType<IndependentComponentA>(component);
            }
        }

        [Test]
        public void CreateBasedOnFactoryMethodReturnTypeWithArguments()
        {
            Chest
                .AddTransient<ComponentWithNonWiredConstructorParameters2>()
                .AddFactory<ISampleFactory>();

            using (var lease = Chest.Fetch<ISampleFactory>())
            {
                ComponentWithNonWiredConstructorParameters2 component = lease.Instance.CreateWithParameters(
                    "a", "b");
                Assert.IsNotNull(component);
                Assert.IsInstanceOfType<ComponentWithNonWiredConstructorParameters2>(component);
            }
        }

        [Test, Pending("We need a policy for this.")]
        public void DoNotAllowSingletonsInFactories()
        {
            throw new NotImplementedException();
        }

        [Test, Pending("Validate() method on the chest")]
        public void EnsureFactoryMethodsCanActuallyReturnSomethingFromTheChest()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void FactoryShouldFindAConstructorThatMatchesTheArgumentNames()
        {
            Chest
                .AddTransient<ComponentWithNonWiredConstructorParametersAndMultipleConstructors>()
                .AddFactory<ISampleFactory>();

            using (var lease = Chest.Fetch<ISampleFactory>())
            {
                ComponentWithNonWiredConstructorParametersAndMultipleConstructors component = lease.Instance.CreateVariant(
                    "a");
                Assert.IsNotNull(component);
                Assert.IsInstanceOfType<ComponentWithNonWiredConstructorParametersAndMultipleConstructors>(component);
            }
        }

        [Test]
        public void FactoryMethodUsingGenerics()
        {
            Chest
                .AddCustom.AllImplementationsOf<IServiceX>().Done
                .AddFactory<ISampleFactory>();
            
            using (var lease = Chest.Fetch<ISampleFactory>())
            {
                IndependentComponentA component = lease.Instance.CreateUsingGenerics<IndependentComponentA>();
                Assert.IsNotNull(component);
            }
        }

        [Test]
        public void FactoryMethodSuppliedTypeParameter()
        {
            Chest
                .AddCustom.AllImplementationsOf<IServiceX>().Done
                .AddFactory<ISampleFactory>();

            using (var lease = Chest.Fetch<ISampleFactory>())
            {
                IServiceX component = lease.Instance
                    .CreateUsingTypeMethod(typeof(IndependentComponentA));
                Assert.IsNotNull(component);
                Assert.IsInstanceOfType<IndependentComponentA>(component);
            }
        }

        [Test]
        public void FactoryReleaseMethod()
        {
            Chest
                .Add<IServiceX, IndependentComponentA>()
                .AddFactory<ISampleFactory>();

            using (var lease = Chest.Fetch<ISampleFactory>())
            {
                IServiceX service = lease.Instance.CreateX();
                lease.Instance.Release(service);
            }
        }

        //[Test]
        //public void RegisterTypeByName()
        //{
        //    Chest
        //        .AddFactory<ISampleFactory>();

        //    using (var lease = Chest.Fetch<ISampleFactory>())
        //    {
        //        lease.Instance.Register<IndependentComponentA>("name");
        //        lease.Instance.
        //    }
        //}
    }
}