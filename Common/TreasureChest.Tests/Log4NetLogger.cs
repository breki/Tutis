using System.Reflection;
using log4net;
using TreasureChest.Logging;

namespace TreasureChest.Tests
{
    public class Log4NetLogger : ILogger
    {
        public void Log(LogEventType eventType, params object[] args)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat(LoggingHelper.FormatLogEvent(eventType, args));
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}