using System;
using System.Globalization;
using System.Text;

namespace FinanceReport.Reporting
{
    public class JsonWriter
    {
         public static void WriteData(StringBuilder stringBuilder, DateTime date, object value)
         {
             stringBuilder.AppendFormat (
                 CultureInfo.InvariantCulture,
                 "[Date.UTC({0}, {1}, {2}), {3}],",
                 date.Year,
                 date.Month - 1,
                 date.Day,
                 value);
         }
    }
}