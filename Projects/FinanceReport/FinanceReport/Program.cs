using System.Text;
using Brejc.Common.FileSystem;
using FinanceReport.Analysis;
using FinanceReport.BackupStorage;
using FinanceReport.DataModel;
using FinanceReport.Razor;
using FinanceReport.Reporting.Models;

namespace FinanceReport
{
    public class Program
    {
        public static void Main (string[] args)
        {
            Database db = FetchLatestFinancistoData ();

            MainReportModel model = new MainReportModel ();

            model.BalancesDailyDiffs = CalcBalancesDiffs.Calc (db);
            model.SpendingByMonth = CalcSpendingByMonth.Calc (db);
            model.SpendingByMonthTrend = model.SpendingByMonth.CalculateTrend (3);
            model.EarningByMonth = CalcEarningByMonth.Calc (db);
            model.EarningByMonthTrend = model.EarningByMonth.CalculateTrend (3);
            model.BalancesDaily = CalcBalances.Calc (model.BalancesDailyDiffs);

            CalcMonthlyBalancesByGroups calcMonthlyBalancesByGroups = new CalcMonthlyBalancesByGroups (db);
            calcMonthlyBalancesByGroups
                .AddGroup ("bencin", 2)
                .AddGroup ("avto - ostali stroški", 3, 16, 17, 18, 6, 30)
                .AddGroup ("hrana in nakupi", 4, 5, 10)
                .AddGroup ("kredit", 19)
                .AddGroup ("stanovanje", 12, 34, 35, 46)
                .AddGroup ("Kozmo", 56)
                .AddGroup ("ostalo");

            model.MonthlySpendingByCategories = calcMonthlyBalancesByGroups.Calc ();

            RenderReport (model, @"\\HOBBIT\Web\finance.html");

            // monthly spending by categories
            // balance trend
            // monthly balance change trend
            // spending vs. earning, trend
            // SM earning, daily, weekly, monthly trend
        }

        public static Database FetchLatestFinancistoData ()
        {
            DropBoxBackupStorage backupStorage = new DropBoxBackupStorage ();
            string financistoBackupFileName = backupStorage.FindLatestBackupFile ();

            FinancistoReader reader = new FinancistoReader ();
            return reader.ReadDatabaseFromZipile (financistoBackupFileName);
        }

        private static void RenderReport (MainReportModel model, string outputFileName)
        {
            WindowsFileSystem fileSystem = new WindowsFileSystem ();
            IReportRenderingEngine reportRenderingEngine = new RazorReportRenderingEngine (fileSystem);
            string body = reportRenderingEngine.RenderView (model);

            fileSystem.WriteFile (outputFileName, body, Encoding.UTF8);
        }
    }
}
