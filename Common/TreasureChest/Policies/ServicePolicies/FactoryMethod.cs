using System;

namespace TreasureChest.Policies.ServicePolicies
{
    public class FactoryMethod : GlobalChestPolicyBase, IComponentCreationMethod
    {
        public FactoryMethod(Func<IChest, object> func)
        {
            this.func = func;
        }

        public object Create()
        {
            return func(Chest);
        }

        private readonly Func<IChest, object> func;
    }
}