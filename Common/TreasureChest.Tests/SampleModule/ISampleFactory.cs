using System;

namespace TreasureChest.Tests.SampleModule
{
    public interface ISampleFactory
    {
        IServiceX CreateX();
        ComponentWithNonWiredConstructorParameters2 CreateWithParameters(
            string arg1,
            string arg2);

        ComponentWithNonWiredConstructorParametersAndMultipleConstructors CreateVariant(string arg1);
        ComponentWithNonWiredConstructorParametersAndMultipleConstructors CreateVariant(string arg1, string arg2);

        TService CreateUsingGenerics<TService>() where TService : IServiceX;
        IServiceX CreateUsingTypeMethod(Type serviceType);

        DisposableComponentA CreateDisposableComponentA();
        IDisposableService CreateIDisposableService();

        void Release(object instance);
    }
}