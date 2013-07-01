using System;
using System.Collections.Generic;
using System.Globalization;

namespace FinanceReport.Reporting
{
    public static class JsonWriter
    {
        public static string ToJson(this KeyValuePair<DateTime, decimal> pair)
        {
            DateTime date = pair.Key;

            return string.Format(
                CultureInfo.InvariantCulture,
                "[Date.UTC({0}, {1}, {2}), {3:F0}],",
                date.Year,
                date.Month - 1,
                date.Day,
                pair.Value);
        }

        public static string ToJson(this KeyValuePair<int, decimal> pair)
        {
            DateTime date = new DateTime (2011, 1, 1).AddMonths (pair.Key);
            return new KeyValuePair<DateTime, decimal>(date, pair.Value).ToJson();
        }

        public static string ToJsonNegative(this KeyValuePair<int, decimal> pair)
        {
            DateTime date = new DateTime (2011, 1, 1).AddMonths (pair.Key);
            return new KeyValuePair<DateTime, decimal>(date, -pair.Value).ToJson();
        }
    }
}