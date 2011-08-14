using System.Globalization;
using System.Text;

namespace TreasureChest.Logging
{
    public static class LoggingHelper
    {
        public static string FormatLogEvent (LogEventType eventType, params object[] args)
        {
            StringBuilder s = new StringBuilder ();
            s.Append (eventType);
            s.Append (" (");

            string comma = null;
            for (int i = 0; i < args.Length; i += 2)
            {
                s.AppendFormat(CultureInfo.InvariantCulture, "{2}{0}='{1}'", args[i], args[i + 1], comma);
                comma = ", ";
            }

            s.Append (")");

            return s.ToString();
        }
    }
}