using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using FinanceReport.Analysis;
using FinanceReport.BackupStorage;
using FinanceReport.DataModel;
using FinanceReport.Reporting;
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
            Database db = FetchLatestFinancistoData();

            AmountByTime balancesDailyDiffs = CalcBalancesDiffs.Calc(db);
            AmountByRange spendingByMonth = CalcSpendingByMonth.Calc (db);
            AmountByRange earningByMonth = CalcEarningByMonth.Calc (db);
            AmountByTime balancesDaily = CalcBalances.Calc(balancesDailyDiffs);

            StringBuilder totalBalanceDataBuilder = new StringBuilder();
            foreach (KeyValuePair<DateTime, decimal> balance in balancesDaily.Amounts)
                JsonWriter.WriteData(totalBalanceDataBuilder, balance.Key, balance.Value);

            StringBuilder spendingDataBuilder = new StringBuilder ();
            foreach (KeyValuePair<int, decimal> entry in spendingByMonth.Amounts)
            {
                DateTime date = new DateTime(2011, 1, 1).AddMonths(entry.Key);
                JsonWriter.WriteData (spendingDataBuilder, date, -entry.Value);
            }

            StringBuilder earningDataBuilder = new StringBuilder ();
            foreach (KeyValuePair<int, decimal> entry in earningByMonth.Amounts)
            {
                DateTime date = new DateTime (2011, 1, 1).AddMonths (entry.Key);
                JsonWriter.WriteData (earningDataBuilder, date, entry.Value);
            }

            CalcMonthlyBalancesByGroups calcMonthlyBalancesByGroups = new CalcMonthlyBalancesByGroups(db);
            calcMonthlyBalancesByGroups
                .AddGroup("bencin", 2)
                .AddGroup("avto - ostali stroški", 3, 16, 17, 18, 6, 30)
                .AddGroup("hrana in nakupi", 4, 5, 10)
                .AddGroup("kredit", 19)
                .AddGroup("stanovanje", 12, 34, 35)
                .AddGroup("ostalo");

            CategoriesRangesAmounts monthlySpendingByCategories = calcMonthlyBalancesByGroups.Calc();

            Hashtable properties = new Hashtable ();
            properties.Add("TotalBalanceData", totalBalanceDataBuilder.ToString());
            properties.Add("SpendingData", spendingDataBuilder.ToString());
            properties.Add("EarningData", earningDataBuilder.ToString());
            properties.Add ("MonthlySpendingByCategories", monthlySpendingByCategories);

            RenderReport(properties);

            // monthly spending by categories
            // balance trend
            // monthly balance change trend
            // spending vs. earning, trend
            // SM earning, daily, weekly, monthly trend
        }

        public static Database FetchLatestFinancistoData()
        {
            DropBoxBackupStorage backupStorage = new DropBoxBackupStorage();
            string financistoBackupFileName = backupStorage.FindLatestBackupFile();

            FinancistoReader reader = new FinancistoReader ();
            return reader.ReadDatabaseFromZipile(financistoBackupFileName);
        }

        private static void RenderReport(Hashtable properties)
        {
            VelocityEngine velocity = new VelocityEngine();
            velocity.SetProperty(RuntimeConstants.RESOURCE_LOADER, "file");
            velocity.Init();

            Template template = velocity.GetTemplate("Reporting/ReportTemplates/index.htm.vm");

            using (Stream stream = File.Open("Reporting/ReportTemplates/index.htm", FileMode.Create))
            {
                using (TextWriter writer = new StreamWriter(stream))
                {
                    VelocityContext velocityContext = new VelocityContext(properties);
                    template.Merge(velocityContext, writer);
                }
            }
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}