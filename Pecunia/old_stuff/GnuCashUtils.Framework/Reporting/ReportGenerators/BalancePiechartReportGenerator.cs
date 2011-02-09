using System;
using System.Collections.Generic;
using ZedGraph;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GnuCashUtils.Framework.Reporting.ReportGenerators
{
    public abstract class BalancePiechartReportGenerator 
    {
        protected GeneratedReport Generate (
            Book book, 
            ReportParameters parameters, 
            string reportTitle,
            string imageFileName,
            AccountType accountType, 
            DateTime? startTime, 
            DateTime? endTime)
        {
            GeneratedReport generatedReport = new GeneratedReport ();

            ZedGraphControl zedGraph = ReportGenerationHelper.CreateGraph ();
            zedGraph.GraphPane.TitleGap = 0;

            // first calculate all totals
            IDictionary<Guid, AccountBalance> allAccountsTotals =
                book.CalculateAccountsBalances (startTime, endTime, parameters.BaseCurrency.FullId);

            // now find the root asset account
            Account rootAssetAccount = book.FindRootAccountForType (accountType);

            AccountBalance totalBalance = allAccountsTotals[rootAssetAccount.Id];

            // add all root's children accounts
            List<AccountBalance> balancesToShow = new List<AccountBalance> ();

            foreach (Account assetAccount in rootAssetAccount.ChildAccounts)
                balancesToShow.Add (allAccountsTotals[assetAccount.Id]);

            // sort the balances descendingly
            balancesToShow.Sort ();

            // add balances into the pie
            int i;
            for (i = 0; i < balancesToShow.Count; i++)
            {
                AccountBalance balance = balancesToShow[i];

                // all balances which are too small to show individually, show in group
                if (balance.Balance / totalBalance.Balance < 0.01m)
                    break;

                AddPieSlice (zedGraph, balance.Balance, 
                    parameters.FillPalette.FindColorForObject (ReportParameters.ObjectTypeAccount, balance.Account.Id.ToString ()), 
                    balance.Account.Name, parameters);
            }

            if (i < balancesToShow.Count)
            {
                decimal otherBalances = 0;
                for (; i < balancesToShow.Count; i++)
                    if (balancesToShow[i].Balance.HasValue)
                        otherBalances += balancesToShow[i].Balance.Value;

                AddPieSlice (zedGraph, otherBalances,
                    parameters.FillPalette.FindColorForObject (ReportParameters.ObjectTypeAccount, "rest"), 
                    "rest", parameters);
            }

            zedGraph.GraphPane.Title.Text = reportTitle;

            zedGraph.GraphPane.Legend.IsVisible = false;

            zedGraph.AxisChange ();

            generatedReport.ImageFileName = Path.Combine (parameters.ReportDirectory, imageFileName);

            using (Image image = zedGraph.GraphPane.GetImage (parameters.ImageWidth, parameters.ImageHeight, 10))
            {
                image.Save (generatedReport.ImageFileName, ImageFormat.Png);

                return generatedReport;
            }
        }

        [CLSCompliant(false)]
        static protected void AddPieSlice (ZedGraphControl zedGraph, decimal? value, int colorRgb, string text, ReportParameters parameters)
        {
            if (value.HasValue)
            {
                PieItem item = zedGraph.GraphPane.AddPieSlice ((double)value,
                    ReportGenerationHelper.GetColor (colorRgb),
                    ReportGenerationHelper.GetAdjustedColor (colorRgb, 0.66), 135f,
                    0, text);
                item.LabelType = PieLabelType.Name_Value_Percent;
                item.PercentDecimalDigits = 1;
                item.ValueDecimalDigits = 0;
                item.Border.IsVisible = false;
                item.LabelDetail.FontSpec.Size = 8;
            }
        }
    }
}
