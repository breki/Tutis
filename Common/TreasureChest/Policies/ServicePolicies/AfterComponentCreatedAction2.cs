using System;

namespace TreasureChest.Policies.ServicePolicies
{
    public class AfterComponentCreatedAction2 : GlobalChestPolicyBase, IAfterComponentCreatedAction
    {
        public AfterComponentCreatedAction2(Action<IChest, object> action)
        {
            this.action = action;
        }

        public void AfterCreated(object instance)
        {
            action(Chest, instance);
        }

        private readonly Action<IChest, object> action;
    }
}