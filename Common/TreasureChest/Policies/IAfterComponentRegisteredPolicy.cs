namespace TreasureChest.Policies
{
    public interface IAfterComponentRegisteredPolicy : IGlobalChestPolicy, IMultipleInstancePolicy
    {
        void AfterRegistered(ServiceRegistration registration);
    }
}