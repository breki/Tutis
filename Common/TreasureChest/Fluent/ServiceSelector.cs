using System;
using System.Diagnostics.CodeAnalysis;
using TreasureChest.Policies;

namespace TreasureChest.Fluent
{
    public class ServiceSelector : IServiceSelector
    {
        public ServiceSelector(
            IChestMaster chest,
            PolicyCollection chestPolicies,
            IServicesRegistry servicesRegistry)
        {
            this.chest = chest;
            this.chestPolicies = chestPolicies;
            this.servicesRegistry = servicesRegistry;
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IServiceAction<T> Service<T> ()
        {
            return new ServiceAction<T>(chest, chestPolicies, servicesRegistry);
        }

        [SuppressMessage ("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public ServiceMultiImplementationsAction<T> AllImplementationsOf<T> ()
        {
            return new ServiceMultiImplementationsAction<T>(chest, chestPolicies, servicesRegistry);
        }

        public IForEachClass ForEachClass(Func<Type, bool> predicate)
        {
            return new ForEachClass(chest, predicate);
        }

        private readonly IChestMaster chest;
        private readonly PolicyCollection chestPolicies;
        private readonly IServicesRegistry servicesRegistry;
    }
}