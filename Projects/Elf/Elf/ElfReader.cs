using System;
using System.Globalization;
using System.IO;

namespace Elf
{
    public class ElfReader : IElfReader
    {
        public LogContents ReadLogFile(string fileName)
        {
            LogContents contents = new LogContents();

            string[] lines = File.ReadAllLines(fileName);

            foreach (string line in lines)
                ParseLine(line, contents);

            return contents;
        }

        private void ParseLine(string line, LogContents contents)
        {
            if (line.StartsWith("#"))
                ParseDirective(line, contents);
            else
                ParseLogEntry(line, contents);
        }

        private void ParseDirective(string line, LogContents contents)
        {
            if (line.StartsWith("#Fields"))
                ParseFields(line, contents);
        }

        private void ParseFields(string line, LogContents contents)
        {
            string fieldsSubstring = line.Substring("#Fields:".Length).Trim();
            string[] fields = fieldsSubstring.Split(' ');
            fieldsContext = new FieldsContext(fields);
        }

        private void ParseLogEntry(string line, LogContents contents)
        {
            string[] columns = line.Split(' ');

            LogEntry entry = new LogEntry();
            for (int i = 0; i < fieldsContext.Fields.Count; i++)
            {
                string field = fieldsContext.Fields[i];
                string value = columns[i];

                ProcessFieldValue(entry, field, value);
            }

            contents.AddLogEntry(entry);
        }

        private static void ProcessFieldValue(LogEntry entry, string field, string value)
        {
            switch (field)
            {
                case "date":
                    entry.Date = DateTime.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "time":
                    entry.Time = TimeSpan.Parse(value, CultureInfo.InvariantCulture);
                    break;
                case "c-ip":
                    entry.ClientIp = ParseFieldValue(value);
                    break;
                case "cs-method":
                    entry.Method = ParseFieldValue (value);
                    break;
                case "cs-uri-stem":
                    entry.UriStem = ParseFieldValue (value);
                    break;
                case "cs-uri-query":
                    entry.UriQuery = ParseFieldValue (value);
                    break;
                case "sc-status":
                    entry.ServerStatus = ParseFieldValue (value);
                    break;
                case "cs(User-Agent)":
                    entry.UserAgent = ParseFieldValue (value);
                    break;
                case "cs(Referrer)":
                    entry.Referrer = ParseFieldValue (value);
                    break;
            }
        }

        private static string ParseFieldValue(string fieldValue)
        {
            if (fieldValue == "-")
                return null;

            return fieldValue;
        }

        private FieldsContext fieldsContext;
    }
}