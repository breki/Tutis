using System;
using System.Collections.Generic;
using System.Globalization;
using FinanceReport.Financisto;
using NUnit.Framework;

namespace FinanceReport.Tests
{
    public class FinancistoReadingTests
    {
        [Test]
        public void Test()
        {
            FinancistoReader reader = new FinancistoReader();
            Database db = reader.ReadDatabaseFromFile(@"D:\MyStuff\Dropbox\Finances\20120315_060236_215.backup");

            DatabaseTable table = db.Tables["transactions"];

            BalanceByTime balances = new BalanceByTime();

            foreach (TableRow row in table.Rows)
            {
                if ((string)row.Values["to_account_id"] == "0")
                {
                    DateTime date = UnixTimeToDateTime((string)row.Values["datetime"]);
                    decimal amount = decimal.Parse((string)row.Values["from_amount"], CultureInfo.InvariantCulture) / 100;
                    balances.AddAmount(date, amount);
                }
            }

            BalanceByTime totalBalances = new BalanceByTime ();
            decimal currentBalance = 0;

            foreach (KeyValuePair<DateTime, decimal> balance in balances.Balances)
            {
                currentBalance += balance.Value;
                totalBalances.AddAmount(balance.Key, currentBalance);
                Console.Out.WriteLine ("{0}: {1}", balance.Key, currentBalance);
            }
        }

        public static DateTime UnixTimeToDateTime (string text)
        {
            double seconds = double.Parse (text, CultureInfo.InvariantCulture);
            return Epoch.AddMilliseconds(seconds);
        }

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}