using System;

namespace TreasureChest.Policies.ServicePolicies
{
    public class VirtualFactory<TService, TDependency> : GlobalChestPolicyBase, IComponentCreationMethod
    {
        public VirtualFactory(IChestMaster chest, Func<TDependency, TService> createFunc)
        {
            this.chest = chest;
            this.createFunc = createFunc;
        }

        public object Create()
        {
            using (Lease<TDependency> lease = chest.Fetch<TDependency>())
            {
                return createFunc(lease.Instance);
            }
        }

        private readonly IChestMaster chest;
        private readonly Func<TDependency, TService> createFunc;
    }
}