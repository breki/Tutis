using System;
using System.Collections.Generic;
using System.Linq;
using TreasureChest.Policies;
using TreasureChest.Policies.ServicePolicies;

namespace TreasureChest.Fluent
{
    public abstract class ServiceActionBase<TService> : IServiceAction<TService>
    {
        public abstract IChestFilling Done
        { 
            get;
        }

        public virtual IServiceAction<TService> FactoryMethod(Func<IChest, TService> createFunc)
        {
            prototypeRegistration.AddPolicy(new FactoryMethod(x => createFunc(x)));
            return this;
        }

        public IServiceAction<TService> ImplementedBy<TImpl>()
        {
            return ImplementedBy(typeof(TImpl));
        }

        public virtual IServiceAction<TService> ImplementedBy(Type implementationType)
        {
            prototypeRegistration.ImplType = implementationType;
            return this;
        }

        public virtual IServiceAction<TService> FactoryMethod<TDependency>(Func<TDependency, TService> createFunc)
        {
            prototypeRegistration.AddPolicy(new VirtualFactory<TService, TDependency>(chest, createFunc));
            return this;
        }

        public IServiceAction<TService> OnCreateDo(Action<TService> action)
        {
            prototypeRegistration.AddPolicy(
                new AfterComponentCreatedAction(x => action((TService)x)));
            return this;
        }

        public IServiceAction<TService> OnCreateDo(Action<IChest, TService> action)
        {
            prototypeRegistration.AddPolicy(
                new AfterComponentCreatedAction2((c, x) => action(chest, (TService)x)));
            return this;
        }

        public IServiceAction<TService> OnDestroyDo(Action<TService> action)
        {
            prototypeRegistration.AddPolicy(
                new BeforeComponentDestroyedAction(x => action((TService)x)));
            return this;
        }

        public IServiceAction<TService> OnRegisterDo(Action<ServiceRegistration> action)
        {
            onRegistrationActions.Add(action);
            return this;
        }

        public abstract IServiceAction<TService> AlsoRegisterFor<TService2>();

        public IServiceAction<TService> Arg(string key, object value)
        {
            prototypeRegistration.Args.Add(key, value);
            return this;
        }

        public virtual IServiceAction<TService> WithLifestyle<TLifestyle>() where TLifestyle : IRegistrationHandler
        {
            lifestyleType = typeof(TLifestyle);
            return this;
        }

        protected ServiceActionBase(
            IChestMaster chest, 
            PolicyCollection chestPolicies,
            IServicesRegistry servicesRegistry)
        {
            this.chest = chest;
            this.chestPolicies = chestPolicies;
            this.servicesRegistry = servicesRegistry;
            prototypeRegistration = new ServiceRegistration();
        }

        protected IChestMaster Chest
        {
            get { return chest; }
        }

        protected PolicyCollection ChestPolicies
        {
            get { return chestPolicies; }
        }

        protected Type LifestyleType
        {
            get { return lifestyleType; }
        }

        protected IServicesRegistry ServicesRegistry
        {
            get { return servicesRegistry; }
        }

        protected ServiceRegistration PrototypeRegistration
        {
            get { return prototypeRegistration; }
        }

        protected IChestFilling AddRegistration(Type serviceType, Type implType)
        {
            if (prototypeRegistration.UsesCustomCreationMethod)
                throw new InvalidOperationException("This method should only be called when custom creation method is not specified.");

            IRegistrationHandler lifestyle = CreateLifestyleBasedOnConfiguration();

            ServiceRegistration registration = prototypeRegistration.CreateFromPrototype(
                serviceType,
                implType,
                lifestyle);
            ServicesRegistry.AddRegistration(registration);
            ExecuteOnRegistrationActions(registration);
            return Chest;
        }

        protected IChestFilling AddRegistration(IEnumerable<Type> serviceTypes, Type implType)
        {
            if (prototypeRegistration.UsesCustomCreationMethod)
                throw new InvalidOperationException("This method should only be called when custom creation method is not specified.");

            IRegistrationHandler lifestyle = CreateLifestyleBasedOnConfiguration();

            ServiceRegistration registration = prototypeRegistration.CreateFromPrototype(
                serviceTypes,
                implType,
                lifestyle);
            ServicesRegistry.AddRegistration(registration);
            ExecuteOnRegistrationActions(registration);
            return Chest;
        }

        protected IChestFilling AddRegistrationWithCustomCreationMethod(IEnumerable<Type> serviceTypes)
        {
            if (!prototypeRegistration.UsesCustomCreationMethod)
                throw new InvalidOperationException("This method should only be called when custom creation method is specified.");

            IRegistrationHandler lifestyle = CreateLifestyleBasedOnConfiguration();

            Type implType = serviceTypes.AsQueryable().First(x => true);
            ServiceRegistration registration = prototypeRegistration.CreateFromPrototype(
                serviceTypes,
                implType,
                lifestyle);
            ServicesRegistry.AddRegistration(registration);
            ExecuteOnRegistrationActions(registration);
            return Chest;
        }

        protected void ExecuteOnRegistrationActions(ServiceRegistration registration)
        {
            foreach (Action<ServiceRegistration> action in onRegistrationActions)
                action(registration);
        }

        private IRegistrationHandler CreateLifestyleBasedOnConfiguration()
        {
            IRegistrationHandler lifestyle;
            if (LifestyleType != null)
                lifestyle = (IRegistrationHandler)Activator.CreateInstance(LifestyleType);
            else
                lifestyle = (IRegistrationHandler)Activator.CreateInstance(Chest.DefaultLifestyleType);
            return lifestyle;
        }

        private readonly IChestMaster chest;
        private readonly PolicyCollection chestPolicies;
        private readonly IServicesRegistry servicesRegistry;
        private Type lifestyleType;
        private ServiceRegistration prototypeRegistration;
        private List<Action<ServiceRegistration>> onRegistrationActions = new List<Action<ServiceRegistration>>();
    }
}