using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using FinanceReport.Financisto;
using log4net;
using NUnit.Framework;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;

namespace FinanceReport.Tests
{
    public class FinancistoReadingTests
    {
        [Test]
        public void Test()
        {
            FinancistoReader reader = new FinancistoReader();
            Database db = reader.ReadDatabaseFromFile (@"D:\MyStuff\Dropbox\Apps\financisto\20130503_202016_178");

            DatabaseTable table = db.Tables["transactions"];

            AmountByTime balances = new AmountByTime();
            //AmountByRange balancesByAccounts = new AmountByRange();
            AmountByRange spendingByMonth = new AmountByRange ();
            AmountByRange earningByMonth = new AmountByRange ();
            AmountByRangeAndTime balancesByAccountsAndDates = new AmountByRangeAndTime();

            foreach (TableRow row in table.Rows)
            {
                DateTime date = UnixTimeToDateTime ((string)row.Values["datetime"]);
                decimal fromAmount = decimal.Parse ((string)row.Values["from_amount"], CultureInfo.InvariantCulture) / 100;
                decimal toAmount = decimal.Parse ((string)row.Values["to_amount"], CultureInfo.InvariantCulture) / 100;
                decimal amount;
                int isTemplate = int.Parse((string)row.Values["is_template"], CultureInfo.InvariantCulture);
                int parentId = int.Parse((string)row.Values["parent_id"], CultureInfo.InvariantCulture);
                if (isTemplate > 0 || parentId > 0)
                    continue;

                int fromAccountId = int.Parse((string)row.Values["from_account_id"]);
                int toAccountId = int.Parse((string)row.Values["to_account_id"]);

                if (toAccountId == 0)
                {
                    balances.AddAmount (date, fromAmount);

                    if (fromAmount < 0)
                        spendingByMonth.AddAmount (GetMonthIndex (date) - GetMonthIndex (new DateTime (2011, 1, 1)), fromAmount);
                    else
                        earningByMonth.AddAmount (GetMonthIndex (date) - GetMonthIndex (new DateTime (2011, 1, 1)), fromAmount);
                }
                else
                {
                    amount = toAmount;
                    balancesByAccountsAndDates.AddAmount(toAccountId, date, amount);
                }

                balancesByAccountsAndDates.AddAmount (fromAccountId, date, fromAmount);

                if (fromAccountId == 2 || toAccountId == 2)
                {
                    if (fromAccountId == 2)
                        amount = fromAmount;
                    else
                        amount = toAmount;

                    if (amount == 0)
                    {
                        int a = 0;
                        a++;
                    }

                    //log.DebugFormat ("{0}: {1}", date, amount);
                }
            }

            AmountByTime accountBalance = balancesByAccountsAndDates.GetByIndex(2);
            decimal currentBalance = 0;
            
            foreach (KeyValuePair<DateTime, decimal> balance in accountBalance.Amounts)
            {
                currentBalance += balance.Value;
                log.DebugFormat ("{0}: {1}", balance.Key, currentBalance);
            }

            AmountByTime totalBalances = new AmountByTime ();
            currentBalance = 0;

            //DateTime lastDate = DateTime.MinValue;
            foreach (KeyValuePair<DateTime, decimal> balance in balances.Amounts)
            {
                DateTime date = balance.Key;

                //if (lastDate > DateTime.MinValue)
                //{
                //    for (int i = 1; ; i++)
                //    {
                //        DateTime midDate = lastDate.AddDays(i);
                //        if (midDate >= date)
                //            break;
                //        totalBalances.SetAmount (midDate, currentBalance);
                //    }
                //}

                currentBalance += balance.Value;
                totalBalances.SetAmount (date, currentBalance);

                //lastDate = date;
            }

            StringBuilder totalBalanceDataBuilder = new StringBuilder();
            foreach (KeyValuePair<DateTime, decimal> balance in totalBalances.Amounts)
                totalBalanceDataBuilder.AppendFormat(
                    CultureInfo.InvariantCulture, 
                    "[Date.UTC({0}, {1}, {2}), {3}],", 
                    balance.Key.Year, 
                    balance.Key.Month-1, 
                    balance.Key.Day, 
                    balance.Value);

            StringBuilder spendingDataBuilder = new StringBuilder ();
            foreach (KeyValuePair<int, decimal> entry in spendingByMonth.Amounts)
            {
                DateTime date = new DateTime(2011, 1, 1).AddMonths(entry.Key);

                spendingDataBuilder.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "[Date.UTC({0}, {1}, {2}), {3}],",
                    date.Year,
                    date.Month - 1,
                    date.Day,
                    -entry.Value);
            }

            StringBuilder earningDataBuilder = new StringBuilder ();
            foreach (KeyValuePair<int, decimal> entry in earningByMonth.Amounts)
            {
                DateTime date = new DateTime (2011, 1, 1).AddMonths (entry.Key);

                earningDataBuilder.AppendFormat (
                    CultureInfo.InvariantCulture,
                    "[Date.UTC({0}, {1}, {2}), {3}],",
                    date.Year,
                    date.Month - 1,
                    date.Day,
                    entry.Value);
            }

            Hashtable properties = new Hashtable ();
            properties.Add("TotalBalanceData", totalBalanceDataBuilder.ToString());
            properties.Add("SpendingData", spendingDataBuilder.ToString());
            properties.Add("EarningData", earningDataBuilder.ToString());

            VelocityEngine velocity = new VelocityEngine ();
            velocity.SetProperty (RuntimeConstants.RESOURCE_LOADER, "file");
            velocity.Init ();

            Template template = velocity.GetTemplate ("Reporting/ReportTemplates/index.htm.vm");

            using (Stream stream = File.Open ("Reporting/ReportTemplates/index.htm", FileMode.Create))
            {
                using (TextWriter writer = new StreamWriter (stream))
                {
                    VelocityContext velocityContext = new VelocityContext (properties);
                    template.Merge (velocityContext, writer);
                }
            }
        }

        private static int GetMonthIndex(DateTime date)
        {
            return date.Year*12 + date.Month;
        }

        public static DateTime UnixTimeToDateTime (string text)
        {
            double seconds = double.Parse (text, CultureInfo.InvariantCulture);
            return Epoch.AddMilliseconds(seconds);
        }

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}