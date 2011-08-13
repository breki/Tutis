using System;
using TreasureChest.Policies;
using TreasureChest.Policies.ServicePolicies;

namespace TreasureChest
{
    public abstract class RegistrationHandlerBase : IRegistrationHandler
    {
        public ServiceRegistration Registration
        {
            get { return registration; }
        }

        public abstract bool RequiresFetchingValidation
        { 
            get;
        }

        public abstract void DestroyAllInstances(PolicyCollection chestPolicies);

        public void Initialize(ServiceRegistration registration)
        {
            this.registration = registration;
        }

        public abstract bool CanBeFetched(
            IChestMaster chest, 
            ResolvingContext context,
            IComponentCreator componentCreator);

        public abstract object GetInstance(
            IChestMaster chest,
            ResolvingContext context,
            IComponentCreator componentCreator);

        public abstract bool MarkInstanceAsReleased(object instance, PolicyCollection chestPolicies);

        public void DestroyInstance(object instance, PolicyCollection chestPolicies)
        {
            if (instance is IChest)
                return;

            foreach (IBeforeComponentDestroyedAction action
                in registration.FindAllPoliciesOf<IBeforeComponentDestroyedAction>())
                action.BeforeDestroyed(instance);

            foreach (IBeforeComponentDestroyedPolicy policy in chestPolicies.FindAllPoliciesOf<IBeforeComponentDestroyedPolicy>())
                policy.BeforeDestroyed(instance);

            if (instance is IDisposable)
                (instance as IDisposable).Dispose();
        }

        protected object GetInstancePrivate(
            IChestMaster chest,
            ResolvingContext context,
            IComponentCreator componentCreator)
        {
            object instance;

            if (!Registration.UsesCustomCreationMethod)
                instance = componentCreator.CreateInstance(
                    registration,
                    context);
            else
            {
                instance = Registration.CreateInstanceUsingCustomCreationMethod(chest);
                context.DependencyGraph.AddInstanceToMap(instance, this, null);
            }

            return instance;
        }

        private ServiceRegistration registration;
    }
}