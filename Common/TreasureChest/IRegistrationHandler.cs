using TreasureChest.Policies;

namespace TreasureChest
{
    public interface IRegistrationHandler
    {
        ServiceRegistration Registration { get; }

        /// <summary>
        /// Gets a value indicating whether this registration needs to go through constructor validation
        /// policy when it is being registered in the chest.
        /// </summary>
        /// <value>
        /// <c>true</c> if it requires constructor validation; otherwise, <c>false</c>.
        /// </value>
        bool RequiresFetchingValidation { get; }

        void DestroyAllInstances(PolicyCollection chestPolicies);

        void Initialize(ServiceRegistration registration);

        bool CanBeFetched(
            IChestMaster chest, 
            ResolvingContext context,
            IComponentCreator componentCreator);

        object GetInstance(
            IChestMaster chest,
            ResolvingContext context,
            IComponentCreator componentCreator);

        bool MarkInstanceAsReleased(object instance, PolicyCollection chestPolicies);
        void DestroyInstance(object instance, PolicyCollection chestPolicies);
    }
}