using System;
using TreasureChest.Policies;

namespace TreasureChest
{
    public interface IChestMaster : IChestFilling
    {
        PolicyCollection ChestPolicies { get; }
        Type DefaultLifestyleType { get; }
        ObjectDependencyGraph DependencyGraph { get; }
        int ObjectsContainedCount { get; }
        ReflectionExplorer ReflectionExplorer { get; }
        IServicesRegistry ServicesRegistry { get; }

        object Fetch(Type serviceType, ResolvingContext resolvingContext);

        /// <summary>
        /// Fetches instance from the specified service registration.
        /// </summary>
        /// <typeparam name="T">The type of the returned instance.</typeparam>
        /// <param name="registration">The service registration to use for fetching.</param>
        /// <returns>A lease to the instance.</returns>
        Lease<T> FetchFromServiceRegistration<T>(ServiceRegistration registration);
        Type GetImplementationType(Type serviceType);
    }
}