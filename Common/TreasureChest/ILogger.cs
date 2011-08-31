namespace TreasureChest
{
    public enum LogEventType
    {
        CreateInstance,
        Fetch,
        FetchAll,
        FetchDependency,
        Reflection,
        RegisterService,
    }

    public interface ILogger
    {
        void Log(LogEventType eventType, params object[] args);
    }
}