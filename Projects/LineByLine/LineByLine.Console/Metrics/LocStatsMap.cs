using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LineByLine.Console.Metrics
{
    public class LocStatsMap
    {
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        public void AddToMap(string fileExtension, ILocStats locStats)
        {
            map.Add(fileExtension.ToLowerInvariant(), locStats);
        }

        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        public ILocStats GetLocStatsForExtension(string fileExtension)
        {
            string extensionLower = fileExtension.ToLowerInvariant();

            // handle situation when the extension is not in the map
            if (false == map.ContainsKey(extensionLower))
                return null;

            return map[extensionLower];
        }

        private Dictionary<string, ILocStats> map = new Dictionary<string, ILocStats>();
    }
}
