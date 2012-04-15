using System;
using System.Diagnostics.CodeAnalysis;
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
            ResolvingContext context,
            IComponentCreator componentCreator);

        public abstract object GetInstance(
            ResolvingContext context,
            IComponentCreator componentCreator);

        public abstract bool MarkInstanceAsReleased(object instance, PolicyCollection chestPolicies);

        [SuppressMessage ("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public void DestroyInstance (object instance, PolicyCollection chestPolicies)
        {
            if (instance is IChest)
                return;

            foreach (IBeforeComponentDestroyedAction action
                in registration.FindAllPoliciesOf<IBeforeComponentDestroyedAction>())
                action.BeforeDestroyed(instance);

            foreach (IBeforeComponentDestroyedPolicy policy in chestPolicies.FindAllPoliciesOf<IBeforeComponentDestroyedPolicy>())
                policy.BeforeDestroyed(instance);

            if (instance is IDisposable)
            {
                (instance as IDisposable).Dispose();
                chest.Logger.Log (LogEventType.DisposeInstance, TreasureChest.Chest.InstanceArgName, instance);
            }

            chest.Logger.Log (LogEventType.DestroyInstance, TreasureChest.Chest.InstanceArgName, instance);
        }

        protected RegistrationHandlerBase (IChestMaster chest)
        {
            this.chest = chest;
        }

        protected IChestMaster Chest
        {
            get { return chest; }
        }

        [SuppressMessage ("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [SuppressMessage ("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        protected object GetInstancePrivate (
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
                
                if (this is SingletonLifestyle)
                    ((SingletonLifestyle)this).MarkAsInstantiated(instance);
            }

            return instance;
        }

        private IChestMaster chest;
        private ServiceRegistration registration;
    }
}