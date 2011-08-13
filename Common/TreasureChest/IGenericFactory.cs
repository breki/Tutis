namespace TreasureChest
{
    public interface IGenericFactory<T> : ILeaseReturning
    {
        Lease<T> Fetch();
    }
}