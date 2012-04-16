using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using TreasureChest.Factories;
using TreasureChest.Fluent;
using TreasureChest.Logging;
using TreasureChest.Policies;

namespace TreasureChest
{
    public class Chest : IChestMaster
    {
        public const string InstanceArgName = "instance";

        public Chest() : this (new NullLogger())
        {
        }

        public Chest(ILogger logger)
        {
            this.logger = logger;
            servicesRegistry = new ServicesRegistry(chestPolicies, logger);
            dependencyGraph = new ObjectDependencyGraph(chestPolicies);
            componentCreator = new ComponentCreator(chestPolicies, logger);

            SetPolicy<PropertyInjectionPolicy>();
            SetPolicy<ConsistentLifestyleRegistrationsConflictValidationPolicy>();
            SetPolicy<DefaultAutowiringConstructorArgumentTypesPolicy>();
            SetPolicy<DefaultConstructionPolicy>();
            SetPolicy<DisposabilityRegistrationValidationPolicy>();
            SetPolicy<DoNotAllowMultipleImplementationsSelectionPolicy>();
            SetPolicy<OverlappingRegistrationsConflictValidationPolicy>();
            SetPolicy<StandardImplementationFilteringPolicy>();

            foreach (IPolicy policy in chestPolicies.EnumerateAllPolicies())
                policy.AssignLogger(logger);

            AddInstance<IChest, IChestMaster>(this);
        }

        public IServiceSelector AddCustom
        {
            get { return new ServiceSelector(this, chestPolicies, servicesRegistry); }
        }

        public ReflectionExplorer ReflectionExplorer
        {
            get { return reflectionExplorer; }
        }

        public Type DefaultLifestyleType
        {
            get { return defaultLifestyle; }
        }

        public ObjectDependencyGraph DependencyGraph
        {
            get { return dependencyGraph; }
        }

        public int ObjectsContainedCount
        {
            get
            {
                // - 1 is for the chest
                lock (this)
                    return dependencyGraph.Count - 1;
            }
        }

        public PolicyCollection ChestPolicies
        {
            get { return chestPolicies; }
        }

        public IServicesRegistry ServicesRegistry
        {
            get { return servicesRegistry; }
        }

        public ILogger Logger
        {
            get { return logger; }
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IChestFilling Add<T> () 
        {
            return Add(typeof(T));
        }

        public IChestFilling Add(Type serviceType)
        {
            lock (this)
            {
                Type implementationType = GetImplementationType(serviceType);

                ServiceRegistration registration = new ServiceRegistration(
                    serviceType,
                    implementationType,
                    (IRegistrationHandler)Activator.CreateInstance(defaultLifestyle, this));
                servicesRegistry.AddRegistration(registration);
                return this;
            }
        }

        public IChestFilling Add(Type serviceType, Type implementationType)
        {
            lock (this)
            {
                ServiceRegistration registration = new ServiceRegistration(
                    serviceType,
                    implementationType,
                    (IRegistrationHandler)Activator.CreateInstance(defaultLifestyle, this));
                servicesRegistry.AddRegistration(registration);
                return this;
            }
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IChestFilling Add<TService, TImpl> ()
            where TImpl : class, TService
        {
            Type serviceType = typeof(TService);
            Type implType = typeof(TImpl);
            ServiceRegistration registration = new ServiceRegistration(
                serviceType, 
                implType, 
                (IRegistrationHandler)Activator.CreateInstance(defaultLifestyle, this));

            servicesRegistry.AddRegistration(registration);
            return this;
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IChestFilling Add<TService1, TService2, TImpl> ()
            where TImpl : class, TService1, TService2
        {
            Type service1Type = typeof(TService1);
            Type service2Type = typeof(TService2);

            Type[] serviceTypes = new[] { service1Type, service2Type };
            Type implType = typeof(TImpl);
            ServiceRegistration registration = new ServiceRegistration(
                serviceTypes, 
                implType, 
                (IRegistrationHandler)Activator.CreateInstance(defaultLifestyle, this));
            servicesRegistry.AddRegistration(registration);
            return this;
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IChestFilling Add<TService1, TService2, TService3, TImpl> () 
            where TImpl : class, TService1, TService2, TService3
        {
            Type service1Type = typeof(TService1);
            Type service2Type = typeof(TService2);
            Type service3Type = typeof(TService3);

            Type[] serviceTypes = new[] { service1Type, service2Type, service3Type };
            Type implType = typeof(TImpl);
            ServiceRegistration registration = new ServiceRegistration(
                serviceTypes,
                implType,
                (IRegistrationHandler)Activator.CreateInstance(defaultLifestyle, this));
            servicesRegistry.AddRegistration(registration);
            return this;
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IChestFilling AddFactory<T> ()
        {
            Type factoryType = typeof(T);
            FactoryProxy proxy = new FactoryProxy(this, factoryType);
            T proxyInstance = (T)proxy.GetTransparentProxy();

            SingletonLifestyle registrationHandler = new SingletonLifestyle(this, proxyInstance);

            ServiceRegistration registration = new ServiceRegistration(
                factoryType,
                factoryType,
                registrationHandler);

            lock (this)
            {
                servicesRegistry.AddRegistration(registration);
                dependencyGraph.AddInstanceToMap(
                    proxyInstance, 
                    registrationHandler,
                    null);
            }

            return this;
        }

        public IChestFilling AddInstance<TService>(TService instance)
        {
            Type serviceType = typeof(TService);
            lock (this)
            {
                try
                {
                    ServiceRegistration registration = new ServiceRegistration(
                        serviceType,
                        serviceType,
                        (IRegistrationHandler)Activator.CreateInstance(defaultLifestyle, this, instance));
                    servicesRegistry.AddRegistration(registration);
                    dependencyGraph.AddInstanceToMap(
                        instance, 
                        registration.RegistrationHandler,
                        null);
                    return this;
                }
                catch (MissingMethodException)
                {
                    throw ChestException("{0} does not allow injecting instances.", defaultLifestyle.Name);
                }
            }
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IChestFilling AddInstance<TService1, TService2> (TService1 instance)
        {
            Type serviceType1 = typeof(TService1);
            Type serviceType2 = typeof(TService2);
            lock (this)
            {
                try
                {
                    IRegistrationHandler registrationHandler = (IRegistrationHandler)
                        Activator.CreateInstance(defaultLifestyle, this, instance);

                    ServiceRegistration registration1 = new ServiceRegistration(
                        serviceType1,
                        serviceType1,
                        registrationHandler);
                    servicesRegistry.AddRegistration(registration1);

                    ServiceRegistration registration2 = new ServiceRegistration(
                        serviceType2,
                        serviceType2,
                        registrationHandler);
                    servicesRegistry.AddRegistration(registration2);

                    dependencyGraph.AddInstanceToMap(
                        instance, 
                        registrationHandler,
                        null);

                    return this;
                }
                catch (MissingMethodException)
                {
                    throw ChestException("{0} does not allow injecting instances.", defaultLifestyle.Name);
                }
            }
        }

        public IChestFilling AddRegistration(ServiceRegistration registration)
        {
            servicesRegistry.AddRegistration(registration);
            return this;
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IChestFilling AddTransient<T> ()
        {
            Type serviceType = typeof(T);

            lock (this)
            {
                Type implementationType = GetImplementationType(serviceType);

                ServiceRegistration registration = new ServiceRegistration(
                    serviceType,
                    implementationType,
                    new TransientLifestyle(this));
                servicesRegistry.AddRegistration(registration);
                return this;
            }
        }

        public IChestFilling AddTransient(Type serviceType)
        {
            lock (this)
            {
                Type implementationType = GetImplementationType(serviceType);

                ServiceRegistration registration = new ServiceRegistration(
                    serviceType,
                    implementationType,
                    new TransientLifestyle(this));
                servicesRegistry.AddRegistration(registration);
                return this;
            }
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IChestFilling AddTransient<TService, TImpl> () where TImpl : class, TService
        {
            Type serviceType = typeof(TService);
            Type implType = typeof(TImpl);
            ServiceRegistration registration = new ServiceRegistration(serviceType, implType, new TransientLifestyle(this));
            servicesRegistry.AddRegistration(registration);
            return this;
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IChestFilling AddTransient<TService1, TService2, TImpl> () 
            where TImpl : class, TService1, TService2
        {
            Type service1Type = typeof(TService1);
            Type service2Type = typeof(TService2);

            Type[] serviceTypes = new[] { service1Type, service2Type };
            Type implType = typeof(TImpl);
            ServiceRegistration registration = new ServiceRegistration(
                serviceTypes,
                implType,
                new TransientLifestyle(this));
            servicesRegistry.AddRegistration(registration);
            return this;
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IChestFilling AddPooled<T> ()
        {
            Type serviceType = typeof(T);

            lock (this)
            {
                Type implementationType = GetImplementationType (serviceType);

                ServiceRegistration registration = new ServiceRegistration (
                    serviceType,
                    implementationType,
                    new PoolLifestyle(this));
                servicesRegistry.AddRegistration (registration);
                return this;
            }
        }

        public IChestFilling AddPooled (Type serviceType)
        {
            lock (this)
            {
                Type implementationType = GetImplementationType (serviceType);

                ServiceRegistration registration = new ServiceRegistration (
                    serviceType,
                    implementationType,
                    new PoolLifestyle(this));
                servicesRegistry.AddRegistration (registration);
                return this;
            }
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IChestFilling AddPooled<TService, TImpl> () where TImpl : class, TService
        {
            Type serviceType = typeof(TService);
            Type implType = typeof(TImpl);
            ServiceRegistration registration = new ServiceRegistration (
                serviceType, implType, new PoolLifestyle (this));
            servicesRegistry.AddRegistration (registration);
            return this;
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IChestFilling AddPooled<TService1, TService2, TImpl> ()
            where TImpl : class, TService1, TService2
        {
            Type service1Type = typeof(TService1);
            Type service2Type = typeof(TService2);

            Type[] serviceTypes = new[] { service1Type, service2Type };
            Type implType = typeof(TImpl);
            ServiceRegistration registration = new ServiceRegistration (
                serviceTypes,
                implType,
                new PoolLifestyle (this));
            servicesRegistry.AddRegistration (registration);
            return this;
        }

        public void Done()
        {
        }

        public Lease<object> Fetch(Type serviceType)
        {
            lock (this)
                return new Lease<object>(
                    this, 
                    Fetch(serviceType, new ResolvingContext(dependencyGraph, serviceType)));
        }

        public Lease<object> Fetch(Type serviceType, IDictionary<string, object> args)
        {
            lock (this)
            {
                ResolvingContext context = new ResolvingContext(dependencyGraph, serviceType, args);
                return new Lease<object>(this, Fetch(serviceType, context));
            }
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public Lease<T> Fetch<T> ()
        {
            Type serviceType = typeof(T);

            lock (this)
                return new Lease<T>(
                    this,
                    (T)Fetch(serviceType, new ResolvingContext(dependencyGraph, serviceType)));
        }

        public object Fetch(Type serviceType, ResolvingContext resolvingContext)
        {
            lock (this)
            {
                if (servicesRegistry.IsServiceRegistered(serviceType))
                {
                    ServiceRegistration registration = servicesRegistry.GetFirstRegistrationForService(
                        serviceType);
                    object instance = registration.RegistrationHandler.GetInstance(
                        resolvingContext,
                        componentCreator);

                    logger.Log (
                        LogEventType.Fetch, 
                        "serviceType", 
                        serviceType, 
                        "stack", 
                        resolvingContext.ResolvingStackToString (),
                        InstanceArgName,
                        instance);

                    return instance;
                }
            }

            string message = string.Format(
                CultureInfo.InvariantCulture,
                "Service {0} is not registered ({1}).",
                serviceType.FullName,
                resolvingContext.ResolvingStackToString());
 
            throw new ChestException(message);
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public Lease<T> FetchFromServiceRegistration<T> (ServiceRegistration registration)
        {
            object instance = registration.RegistrationHandler.GetInstance(
                new ResolvingContext(DependencyGraph, typeof(T)), 
                componentCreator);

            logger.Log (LogEventType.Fetch, "serviceType", typeof(T), InstanceArgName, instance);

            return new Lease<T>(this, (T)instance);
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IEnumerable<T> FetchAll<T> ()
        {
            Type serviceType = typeof(T);

            logger.Log (LogEventType.FetchAll, "serviceType", serviceType);

            lock (this)
            {
                if (!HasService<T>())
                    throw ChestException("Service {0} is not registered.", serviceType.FullName);

                foreach (ServiceRegistration registration 
                    in servicesRegistry.EnumerateRegistrationsForService(serviceType))
                {
                    ResolvingContext context = new ResolvingContext(dependencyGraph, serviceType);

                    try
                    {
                        T instance = (T)registration.RegistrationHandler.GetInstance(
                                            context,
                                            componentCreator);

                        logger.Log (
                            LogEventType.Fetch,
                            "serviceType",
                            serviceType,
                            "stack",
                            context.ResolvingStackToString (),
                            InstanceArgName,
                            instance);

                        yield return instance;
                    }
                    finally
                    {
                        context.PopType();                        
                    }
                }
            }
        }

        public IEnumerable FetchAll(Type serviceType)
        {
            logger.Log (LogEventType.FetchAll, "serviceType", serviceType);

            lock (this)
            {
                if (!HasService(serviceType))
                    throw ChestException("Service {0} is not registered.", serviceType.FullName);

                foreach (ServiceRegistration registration in
                    servicesRegistry.EnumerateRegistrationsForService(serviceType))
                {
                    ResolvingContext context = new ResolvingContext(dependencyGraph, serviceType);
                    context.PushServiceType(serviceType);

                    try
                    {
                        object instance = registration.RegistrationHandler.GetInstance(
                            context,
                            componentCreator);

                        logger.Log (
                            LogEventType.Fetch,
                            "serviceType",
                            serviceType,
                            "stack",
                            context.ResolvingStackToString (),
                            InstanceArgName,
                            instance);

                        yield return instance;
                    }
                    finally
                    {
                        context.PopType();                        
                    }
                }
            }
        }

        public Type GetImplementationType(Type serviceType)
        {
            lock (this)
            {
                Type implementationType;
                if (serviceType.IsInterface)
                {
                    IImplementationFilteringPolicy filteringPolicy 
                        = chestPolicies.FindPolicyOf<IImplementationFilteringPolicy>();
                    IList<Type> implementations = filteringPolicy.FindImplementationsOfTypes(serviceType);
                    IImplementationSelectionPolicy selectionPolicy 
                        = chestPolicies.FindPolicyOf<IImplementationSelectionPolicy>();
                    implementationType = selectionPolicy.SelectImplementation(serviceType, implementations);
                }
                else
                    implementationType = serviceType;
                return implementationType;
            }
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public bool HasService<T> ()
        {
            Type serviceType = typeof(T);
            return HasService(serviceType);
        }

        public bool HasService(Type serviceType)
        {
            return servicesRegistry.IsServiceRegistered(serviceType);
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IChestFilling InstallExtension<TExtension> () where TExtension : IChestExtension, new ()
        {
            return InstallExtension(new TExtension());
        }

        public IChestFilling InstallExtension(IChestExtension extension)
        {
            extension.Initialize(this);
            chestPolicies.AddPolicy(extension);
            return this;
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void AssertExtensionIsInstalled<TExtension> () where TExtension : IChestExtension
        {
            if (false == chestPolicies.HasPoliciesOf<TExtension>())
            {
                throw ChestException(
                    "The required extension {0} is not installed.",
                    typeof(TExtension).FullName);
            }
        }

        public IChestFilling RegisterAssembly(Assembly assembly)
        {
            reflectionExplorer.RegisterAssembly(assembly);
            return this;
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IChestFilling RegisterAssemblyOf<T> ()
        {
            reflectionExplorer.RegisterAssemblyOf<T>();
            return this;
        }

        public void RegisterDependency(object dependencyInstance, object dependentInstance)
        {
            dependencyGraph.RegisterDependency(dependencyInstance, dependentInstance);
        }

        [SuppressMessage ("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public void Return (object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            lock (this)
            {
                // 27.09.2011: this allows calling factory release methods transparently, even on objects that are not in the container
                if (!dependencyGraph.HandlesInstance (instance))
                {
                    logger.Log (LogEventType.ReleaseInstance, InstanceArgName, instance);

                    if (instance is IDisposable)
                    {
                        (instance as IDisposable).Dispose();
                        logger.Log (LogEventType.DisposeInstance, InstanceArgName, instance);
                    }

                    logger.Log (LogEventType.DestroyInstance, InstanceArgName, instance);

                    return;
                }

                IRegistrationHandler instanceHandler = dependencyGraph.GetRegistrationHandlerForInstance(
                    instance);
                if (instanceHandler.MarkInstanceAsReleased (instance, chestPolicies))
                    dependencyGraph.RemoveInstance(instance, true);
            }
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IChestFilling SetDefaultLifestyle<T> ()
            where T : IRegistrationHandler
        {
            defaultLifestyle = typeof(T);
            return this;
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IChestFilling SetPolicy<T> () where T : IGlobalChestPolicy, new ()
        {
            T policy = new T();
            policy.Initialize(this);
            chestPolicies.AddPolicy(policy);
            return this;
        }

        public IChestFilling SetPolicy(IGlobalChestPolicy policy) 
        {
            policy.Initialize(this);
            chestPolicies.AddPolicy(policy);
            return this;
        }

        public static ChestException ChestException (string messageFormat, params object[] args)
        {
            string message = string.Format(
                CultureInfo.InvariantCulture,
                messageFormat,
                args);
            return new ChestException(message);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources         

                if (disposing)
                {
                    lock (this)
                    {
                        // clean managed resources
                        dependencyGraph.DestroyAllInstances(servicesRegistry);
                    }
                }

                disposed = true;
            }
        }

        private PolicyCollection chestPolicies = new PolicyCollection();
        private IComponentCreator componentCreator;
        private Type defaultLifestyle = typeof(SingletonLifestyle);
        private bool disposed;
        private ObjectDependencyGraph dependencyGraph;
        private ReflectionExplorer reflectionExplorer = new ReflectionExplorer();
        private IServicesRegistry servicesRegistry;
        private ILogger logger = new NullLogger();
    }
}