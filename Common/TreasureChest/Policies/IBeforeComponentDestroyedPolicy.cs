namespace TreasureChest.Policies
{
    public interface IBeforeComponentDestroyedPolicy : IGlobalChestPolicy, IMultipleInstancePolicy
    {
        void BeforeDestroyed(object instance);
    }
}