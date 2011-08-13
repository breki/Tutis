namespace TreasureChest.Policies
{
    public interface IGlobalChestPolicy : IPolicy
    {
        void Initialize(IChestMaster chest);
    }
}