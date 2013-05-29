using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace FinanceReport.BackupStorage
{
    public class DropBoxBackupStorage : IBackupStorage
    {
        public string FindLatestBackupFile()
        {
            string latestFileName = null;
            DateTime? latestBackupTime = null;

            foreach (string fileName in Directory.GetFiles (BackupStorageDir, "*.backup"))
            {
                DateTime? backupTime = ParseBackupTime(fileName);
                if (!backupTime.HasValue)
                    continue;

                if (!latestBackupTime.HasValue || backupTime > latestBackupTime)
                {
                    latestBackupTime = backupTime;
                    latestFileName = fileName;
                }
            }

            return latestFileName;
        }

        private static DateTime? ParseBackupTime(string fileName)
        {
            Match match = filePattern.Match(Path.GetFileName(fileName));

            if (!match.Success)
                return null;

            int year = int.Parse(match.Groups["year"].Value, CultureInfo.InvariantCulture);
            int month = int.Parse(match.Groups["month"].Value, CultureInfo.InvariantCulture);
            int day = int.Parse(match.Groups["day"].Value, CultureInfo.InvariantCulture);
            int hour = int.Parse(match.Groups["hour"].Value, CultureInfo.InvariantCulture);
            int minute = int.Parse(match.Groups["minute"].Value, CultureInfo.InvariantCulture);
            int second = int.Parse(match.Groups["second"].Value, CultureInfo.InvariantCulture);
            return new DateTime(year, month, day, hour, minute, second);
        }

        private const string BackupStorageDir = @"D:\MyStuff\Dropbox\Apps\financisto";
        private static Regex filePattern = new Regex ("^(?<year>[0-9]{4})(?<month>[0-9]{2})(?<day>[0-9]{2})_(?<hour>[0-9]{2})(?<minute>[0-9]{2})(?<second>[0-9]{2})");
    }
}