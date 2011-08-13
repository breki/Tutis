using System;

namespace TreasureChest.Policies.ServicePolicies
{
    public class BeforeComponentDestroyedAction : GlobalChestPolicyBase, IBeforeComponentDestroyedAction
    {
        public BeforeComponentDestroyedAction(Action<object> action)
        {
            this.action = action;
        }

        public void BeforeDestroyed(object instance)
        {
            action(instance);
        }

        private readonly Action<object> action;
    }
}