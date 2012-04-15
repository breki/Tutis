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
                object argName = args[i];
                object argValue = args[i + 1];

                if ((string)argName == Chest.InstanceArgName)
                    argValue = string.Format(
                        CultureInfo.InvariantCulture, "{0} ({1})", argValue.GetType().FullName, argValue.GetHashCode());

                s.AppendFormat(CultureInfo.InvariantCulture, "{2}{0}='{1}'", argName, argValue, comma);
                comma = ", ";
            }

            s.Append (")");

            return s.ToString();
        }
    }
}