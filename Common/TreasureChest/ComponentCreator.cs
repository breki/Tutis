using System;
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
            ServiceRegistration serviceRegistration,
            ResolvingContext context)
        {
            object instance = chestPolicies.FindPolicyOf<IConstructionPolicy>()
                .CreateInstance(serviceRegistration, context);

            foreach (IAfterComponentCreatedAction policy in serviceRegistration.FindAllPoliciesOf<IAfterComponentCreatedAction>())
                policy.AfterCreated(instance);

            return instance;
        }

        private readonly PolicyCollection chestPolicies;
        private readonly ILogger logger;
    }
}