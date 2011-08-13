using System;

namespace TreasureChest.Fluent
{
    public interface IServiceAction<TService>
    {
        IChestFilling Done { get; }

        IServiceAction<TService> AlsoRegisterFor<TService2>();
        IServiceAction<TService> Arg(string key, object value);
        IServiceAction<TService> FactoryMethod(Func<IChest, TService> createFunc);
        IServiceAction<TService> FactoryMethod<TDependency>(Func<TDependency, TService> createFunc);
        IServiceAction<TService> ImplementedBy<TImpl>();
        IServiceAction<TService> ImplementedBy(Type implementationType);
        IServiceAction<TService> OnCreateDo(Action<TService> action);
        IServiceAction<TService> OnCreateDo(Action<IChest, TService> action);
        IServiceAction<TService> OnDestroyDo(Action<TService> action);
        IServiceAction<TService> OnRegisterDo(Action<ServiceRegistration> action);
        IServiceAction<TService> WithLifestyle<TLifestyle>() where TLifestyle : IRegistrationHandler;
    }
}