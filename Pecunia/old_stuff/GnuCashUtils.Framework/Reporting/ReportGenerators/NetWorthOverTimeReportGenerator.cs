using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using ZedGraph;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace GnuCashUtils.Framework.Reporting.ReportGenerators
{
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OverTime")]
    public class NetWorthOverTimeReportGenerator : IReport
    {
        #region IReport Members

        public GeneratedReport Generate (Book book, ReportParameters parameters)
        {
            GeneratedReport generatedReport = new GeneratedReport ();

            ZedGraphControl zedGraph = ReportGenerationHelper.CreateGraph ();

            zedGraph.GraphPane.LineType = LineType.Stack;

            CurveList curveList = new CurveList ();

            // create line items for all of the terminal asset accounts to be shown
            Dictionary<Guid, LineItem> series = new Dictionary<Guid, LineItem> ();
            foreach (Account account in parameters.EnumerateAccounts ())
            {
                LineItem lineItem = new LineItem (account.Name);
                lineItem.Line.StepType = StepType.ForwardStep;
                lineItem.Line.Style = System.Drawing.Drawing2D.DashStyle.Solid;
                lineItem.Symbol.Type = SymbolType.None;
                lineItem.Color = ReportGenerationHelper.GetColor (
                    parameters.FillPalette.FindColorForObject (ReportParameters.ObjectTypeAccount, account.Id.ToString ()));
                lineItem.Line.Width = 3;
                lineItem.Line.Fill = new Fill (ReportGenerationHelper.GetColor (
                    parameters.FillPalette.FindColorForObject (ReportParameters.ObjectTypeAccount, account.Id.ToString ())),
                    Color.White, 135f);
                lineItem.Line.IsAntiAlias = true;
                series.Add (account.Id, lineItem);
            }

            // if the start date has not been set, find the first date
            DateTime? startTime = parameters.StartTime;
            if (false == startTime.HasValue)
                startTime = book.FindFirstTransactionDatePosted ();

            // if the end date has not been set, use the current date
            DateTime? endTime = parameters.EndTime;
            if (false == endTime.HasValue)
                endTime = DateTime.Now.AddDays (1);

            // now calculate balance for each day
            for (DateTime day = startTime.Value.Date; day < endTime.Value.Date; day = day.AddDays (1))
            {
                // calculate balances for that day
                IDictionary<Guid, AccountBalance> balances =
                    book.CalculateAccountsBalances (null, day, parameters.BaseCurrency.FullId);

                // now add points for those balances
                foreach (Guid accountId in series.Keys)
                {
                    Account account = book.GetAccountById (accountId);

                    AccountBalance balance = balances[account.Id];
                    if (balance.Balance.HasValue)
                        series[account.Id].AddPoint (day.ToOADate (), (double)balance.Balance.Value);
                }
            }

            // now add all series to the curvelist (only if they actually contain any points!)
            foreach (LineItem lineItem in series.Values)
                if (lineItem.Points.Count > 0)
                    curveList.Add (lineItem);

            zedGraph.GraphPane.CurveList = curveList;

            zedGraph.GraphPane.Title.Text = "Net Worth Over Time";

            zedGraph.GraphPane.XAxis.Title.Text = "Time";
            zedGraph.GraphPane.XAxis.Type = AxisType.Date;

            zedGraph.GraphPane.XAxis.MajorGrid.IsVisible = true;
            zedGraph.GraphPane.XAxis.MajorGrid.Color = Color.Gray;
            zedGraph.GraphPane.XAxis.Title.FontSpec.Size = 8;
            zedGraph.GraphPane.XAxis.Scale.FontSpec.Size = 8;
            zedGraph.GraphPane.XAxis.Scale.MinGrace = 0;
            zedGraph.GraphPane.XAxis.Scale.MaxGrace = 0;

            zedGraph.GraphPane.YAxis.Title.Text = String.Format (System.Globalization.CultureInfo.InvariantCulture,
                "Net Worth");
            zedGraph.GraphPane.YAxis.Title.FontSpec.Size = 8;
            zedGraph.GraphPane.YAxis.MajorGrid.IsVisible = true;
            zedGraph.GraphPane.YAxis.MajorGrid.Color = Color.Gray;
            zedGraph.GraphPane.YAxis.Scale.FontSpec.Size = 8;
            zedGraph.GraphPane.YAxis.Type = AxisType.Linear;
            zedGraph.GraphPane.YAxis.Scale.Format = "n0";
            zedGraph.GraphPane.YAxis.Scale.MaxGrace = 0.01;

            zedGraph.AxisChange ();

            generatedReport.ImageFileName = Path.Combine (parameters.ReportDirectory, "NetWorthOverTime.png");

            using (Image image = zedGraph.GraphPane.GetImage (parameters.ImageWidth, parameters.ImageHeight, 10))
            {
                image.Save (generatedReport.ImageFileName, ImageFormat.Png);

                return generatedReport;
            }
        }

        #endregion
    }
}
