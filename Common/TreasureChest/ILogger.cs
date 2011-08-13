using System;

namespace TreasureChest
{
    public enum LogEventType
    {
        CreateInstance,
        Fetch,
        FetchAll,
        FetchDependency
    }

    public interface ILogger
    {
        void Log(LogEventType eventType, params object[] args);
    }

    public class NullLogger : ILogger
    {
        public void Log(LogEventType eventType, params object[] args)
        {
        }
    }
}