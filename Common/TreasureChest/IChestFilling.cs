using System;
using System.Reflection;
using TreasureChest.Fluent;
using TreasureChest.Policies;

namespace TreasureChest
{
    public interface IChestFilling : IChest
    {
        IServiceSelector AddCustom { get; }

        IChestFilling Add<T>();
        IChestFilling Add(Type serviceType);
        IChestFilling Add(Type serviceType, Type implementationType);
        IChestFilling Add<TService, TImpl>() where TImpl : class, TService;

        IChestFilling Add<TService1, TService2, TImpl>()
            where TImpl : class, TService1, TService2;
        IChestFilling Add<TService1, TService2, TService3, TImpl>()
            where TImpl : class, TService1, TService2, TService3;

        IChestFilling AddFactory<T>();

        IChestFilling AddInstance<TService>(TService instance);
        IChestFilling AddInstance<TService1, TService2>(TService1 instance);

        IChestFilling AddRegistration(ServiceRegistration registration);

        IChestFilling AddTransient<T>();
        IChestFilling AddTransient(Type serviceType);
        IChestFilling AddTransient<TService, TImpl>() where TImpl : class, TService;
        IChestFilling AddTransient<TService1, TService2, TImpl>() where TImpl : class, TService1, TService2;

        void Done();

        bool HasService<T>();
        bool HasService(Type serviceType);

        IChestFilling InstallExtension<TExtension>() where TExtension : IChestExtension, new();
        IChestFilling InstallExtension(IChestExtension extension);
        void AssertExtensionIsInstalled<TExtension>() where TExtension : IChestExtension;

        IChestFilling RegisterAssembly(Assembly assembly);
        IChestFilling RegisterAssemblyOf<T>();

        IChestFilling SetDefaultLifestyle<T>() where T : IRegistrationHandler;
        IChestFilling SetPolicy<T>() where T : IGlobalChestPolicy, new();
        IChestFilling SetPolicy(IGlobalChestPolicy policy);
    }
}