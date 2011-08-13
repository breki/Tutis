using System;

namespace TreasureChest.Policies.ServicePolicies
{
    public class AfterComponentCreatedAction : GlobalChestPolicyBase, IAfterComponentCreatedAction
    {
        public AfterComponentCreatedAction(Action<object> action)
        {
            this.action = action;
        }

        public void AfterCreated(object instance)
        {
            action(instance);
        }

        private readonly Action<object> action;
    }
}