using System;
using System.Diagnostics.CodeAnalysis;
using TreasureChest.Policies;
using TreasureChest.Policies.ServicePolicies;

namespace TreasureChest
{
    public class ComponentCreator : IComponentCreator
    {
        public ComponentCreator(PolicyCollection chestPolicies, ILogger logger)
        {
            this.chestPolicies = chestPolicies;
            this.logger = logger;
        }

        public bool CanBeCreated(Type type, ResolvingContext context)
        {
            return chestPolicies.FindPolicyOf<IConstructionPolicy>().CanBeCreated(type, context);
        }

        public object CreateInstance(
            ServiceRegistration registration,
            ResolvingContext context)
        {
            object instance = chestPolicies.FindPolicyOf<IConstructionPolicy>()
                .CreateInstance(registration, context);

            foreach (IAfterComponentCreatedAction policy in registration.FindAllPoliciesOf<IAfterComponentCreatedAction>())
                policy.AfterCreated(instance);

            return instance;
        }

        private readonly PolicyCollection chestPolicies;
        [SuppressMessage ("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private readonly ILogger logger;
    }
}