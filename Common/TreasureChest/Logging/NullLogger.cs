using System.Globalization;
using System.Text;

namespace TreasureChest.Logging
{
    public class NullLogger : ILogger
    {
        public void Log(LogEventType eventType, params object[] args)
        {
        }
    }
}