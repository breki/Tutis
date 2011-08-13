namespace Clavis
{
    public interface IClavisStore : IClavisSessionFactory
    {
        void Initialize(IClavisFile clavisFile, string storeName);
        void CreateStore(IClavisSession session);
        void DestroyStore(IClavisSession session);
    }
}