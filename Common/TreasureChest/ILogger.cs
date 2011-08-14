namespace TreasureChest
{
    public enum LogEventType
    {
        CreateInstance,
        Fetch,
        FetchAll,
        FetchDependency,
        RegisterService,
    }

    public interface ILogger
    {
        void Log(LogEventType eventType, params object[] args);
    }
}