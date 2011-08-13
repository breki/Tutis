namespace TreasureChest.Policies
{
    public interface IAfterComponentCreatedPolicy : IGlobalChestPolicy, IMultipleInstancePolicy
    {
        void AfterCreated(object instance, IRegistrationHandler registrationHandler);
    }
}