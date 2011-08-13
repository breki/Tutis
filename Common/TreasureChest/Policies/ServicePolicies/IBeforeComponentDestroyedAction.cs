namespace TreasureChest.Policies.ServicePolicies
{
    public interface IBeforeComponentDestroyedAction : IServicePolicy, IMultipleInstancePolicy
    {
        void BeforeDestroyed(object instance);
    }
}