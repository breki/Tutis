using System;

namespace TreasureChest.Logging
{
    public class ConsoleLogger : ILogger
    {
        public void Log(LogEventType eventType, params object[] args)
        {
            Console.Out.WriteLine(LoggingHelper.FormatLogEvent(eventType, args));
        }
    }
}