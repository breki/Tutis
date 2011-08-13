namespace TreasureChest.Policies.ServicePolicies
{
    public interface IAfterComponentCreatedAction : IServicePolicy, IMultipleInstancePolicy
    {
        void AfterCreated(object instance);
    }
}