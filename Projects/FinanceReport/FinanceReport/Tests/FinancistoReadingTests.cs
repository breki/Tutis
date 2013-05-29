using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using FinanceReport.BackupStorage;
using FinanceReport.Financisto;
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
            Database db = FetchLatesFinancistoData();

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

            Hashtable properties = new Hashtable ();
            properties.Add("TotalBalanceData", totalBalanceDataBuilder.ToString());
            properties.Add("SpendingData", spendingDataBuilder.ToString());
            properties.Add("EarningData", earningDataBuilder.ToString());

            RenderReport(properties);
        }

        private static Database FetchLatesFinancistoData()
        {
            DropBoxBackupStorage backupStorage = new DropBoxBackupStorage();
            string financistoBackupFileName = backupStorage.FindLatestBackupFile();

            Database db;

            using (Stream stream = File.OpenRead(financistoBackupFileName))
            using (GZipStream gzstream = new GZipStream(stream, CompressionMode.Decompress))
            using (MemoryStream memStream = new MemoryStream())
            {
                byte[] buffer = new byte[10000];

                while (true)
                {
                    int actuallyRead = gzstream.Read(buffer, 0, buffer.Length);
                    if (actuallyRead == 0)
                        break;

                    memStream.Write(buffer, 0, actuallyRead);
                }

                memStream.Seek(0, SeekOrigin.Begin);
                FinancistoReader reader = new FinancistoReader();
                db = reader.ReadDatabaseFromStream(memStream);
            }
            return db;
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