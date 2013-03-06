using System.Collections.Generic;

namespace Elf
{
    public class LogContents
    {
        public void AddLogEntry(LogEntry entry)
        {
            entries.Add(entry);
        }

        public IList<LogEntry> Entries
        {
            get { return entries; }
        }

        private List<LogEntry> entries = new List<LogEntry>();
    }
}