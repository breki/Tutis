namespace TreasureChest.Policies.ServicePolicies
{
    public interface IComponentCreationMethod : IServicePolicy, ISingleInstancePolicy
    {
        object Create();
    }
}