namespace TreasureChest
{
    public enum LogEventType
    {
        CreateInstance,
        Fetch,
        FetchAll,
        FetchDependency,
        RegisterService,
        ReleaseInstance,
        DisposeInstance,
        DestroyInstance,
        Informational,
        Error,
    }

    public interface ILogger
    {
        void Log(LogEventType eventType, params object[] args);
    }
}