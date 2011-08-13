namespace NewDock2
{
    public interface IPaintingContext
    {
        T Get<T>(string name);
        void Set(string name, object value);
    }
}