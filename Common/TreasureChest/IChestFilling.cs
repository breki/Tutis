using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TreasureChest.Fluent;
using TreasureChest.Policies;

namespace TreasureChest
{
    public interface IChestFilling : IChest
    {
        IServiceSelector AddCustom { get; }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IChestFilling Add<T> ();
        IChestFilling Add(Type serviceType);
        IChestFilling Add(Type serviceType, Type implementationType);
        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IChestFilling Add<TService, TImpl> () where TImpl : class, TService;

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IChestFilling Add<TService1, TService2, TImpl> ()
            where TImpl : class, TService1, TService2;
        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IChestFilling Add<TService1, TService2, TService3, TImpl> ()
            where TImpl : class, TService1, TService2, TService3;

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IChestFilling AddFactory<T> ();

        IChestFilling AddInstance<TService>(TService instance);
        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IChestFilling AddInstance<TService1, TService2> (TService1 instance);

        IChestFilling AddRegistration(ServiceRegistration registration);

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IChestFilling AddTransient<T> ();
        IChestFilling AddTransient(Type serviceType);
        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IChestFilling AddTransient<TService, TImpl> () where TImpl : class, TService;
        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IChestFilling AddTransient<TService1, TService2, TImpl> () where TImpl : class, TService1, TService2;

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IChestFilling AddPooled<T> ();
        IChestFilling AddPooled (Type serviceType);
        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IChestFilling AddPooled<TService, TImpl> () where TImpl : class, TService;
        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IChestFilling AddPooled<TService1, TService2, TImpl> () where TImpl : class, TService1, TService2;

        void Done();

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        bool HasService<T> ();
        bool HasService(Type serviceType);

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IChestFilling InstallExtension<TExtension> () where TExtension : IChestExtension, new ();
        IChestFilling InstallExtension(IChestExtension extension);
        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        void AssertExtensionIsInstalled<TExtension> () where TExtension : IChestExtension;

        IChestFilling RegisterAssembly(Assembly assembly);
        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IChestFilling RegisterAssemblyOf<T> ();

        void RegisterDependency(object dependencyInstance, object dependentInstance);

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IChestFilling SetDefaultLifestyle<T> () where T : IRegistrationHandler;
        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        IChestFilling SetPolicy<T> () where T : IGlobalChestPolicy, new ();
        IChestFilling SetPolicy(IGlobalChestPolicy policy);
    }
}