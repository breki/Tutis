using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TreasureChest.Policies;

namespace TreasureChest.Fluent
{
    public class ServiceMultiImplementationsAction<TService>
    {
        public ServiceMultiImplementationsAction(
            IChestMaster chest,
            PolicyCollection chestPolicies,
            IServicesRegistry servicesRegistry)
        {
            this.chest = chest;
            this.chestPolicies = chestPolicies;
            this.servicesRegistry = servicesRegistry;
            prototypeRegistration = new ServiceRegistration();
        }

        public IChestFilling Done
        {
            get
            {
                Type serviceType = typeof(TService);
                foreach (Type implType
                    in chest.ReflectionExplorer.FindImplementationsOfTypes(serviceType))
                {
                    AddRegistration(new[] { serviceType, implType }, implType);
                }

                return chest;
            }
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public ServiceMultiImplementationsAction<TService> WithLifestyle<TLifestyle> () 
            where TLifestyle : IRegistrationHandler
        {
            lifestyleType = typeof(TLifestyle);
            return this;
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
            servicesRegistry.AddRegistration(registration);
            ExecuteOnRegistrationActions(registration);
            return chest;
        }

        protected void ExecuteOnRegistrationActions(ServiceRegistration registration)
        {
            foreach (Action<ServiceRegistration> action in onRegistrationActions)
                action(registration);
        }

        private IRegistrationHandler CreateLifestyleBasedOnConfiguration()
        {
            IRegistrationHandler lifestyle;
            if (lifestyleType != null)
                lifestyle = (IRegistrationHandler)Activator.CreateInstance(lifestyleType, chest);
            else
                lifestyle = (IRegistrationHandler)Activator.CreateInstance(chest.DefaultLifestyleType, chest);
            return lifestyle;
        }

        private readonly IChestMaster chest;
        [SuppressMessage ("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private readonly PolicyCollection chestPolicies;
        private readonly IServicesRegistry servicesRegistry;
        private Type lifestyleType;
        private ServiceRegistration prototypeRegistration;
        private List<Action<ServiceRegistration>> onRegistrationActions = new List<Action<ServiceRegistration>>();
    }
}