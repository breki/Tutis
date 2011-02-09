using System;
using System.Collections.Generic;
using System.Text;
using ZedGraph;
using System.Drawing;
using System.Drawing.Imaging;
using log4net;
using System.IO;

namespace GnuCashUtils.Framework.Reporting.ReportGenerators
{
    public class BalanceBarchartReportGenerator : IReport
    {
        public GeneratedReport Generate (
            Book book,
            ReportParameters parameters)
        {
            GeneratedReport generatedReport = new GeneratedReport ();

            ZedGraphControl zedGraph = ReportGenerationHelper.CreateGraph ();
            zedGraph.GraphPane.TitleGap = 0;

            // create line items for all of the terminal asset accounts to be shown
            Dictionary<Guid, BarItem> series = new Dictionary<Guid, BarItem> ();
            foreach (Account account in parameters.EnumerateAccounts ())
            {
                BarItem item = new BarItem (account.Name);
                item.Tag = account;
                item.IsOverrideOrdinal = true;
                series.Add (account.Id, item);
            }

            DateTime firstSegmentStartTime = ReportGenerationHelper.FindSegmentStartTime (parameters.StartTime.Value, parameters.Timescale);
            DateTime segmentStartTime = firstSegmentStartTime;

            List<string> textLabels = new List<string>();

            DateTime endTime;
            if (parameters.EndTime.HasValue)
                endTime = parameters.EndTime.Value;
            else
                endTime = DateTime.Now;

            while (segmentStartTime < endTime)
            {
                DateTime segmentEndTime = ReportGenerationHelper.FindSegmentEndTime (segmentStartTime, parameters.Timescale);
                string segmentName = ReportGenerationHelper.GetSegmentName (segmentStartTime, segmentEndTime, parameters.Timescale);

                textLabels.Add (segmentName);

                // calculate all balances for the segment
                IDictionary<Guid, AccountBalance> balances =
                    book.CalculateAccountsBalances (segmentStartTime, segmentEndTime, parameters.BaseCurrency.FullId);

                // add all root's children accounts
                List<AccountBalance> balancesToShow = new List<AccountBalance> ();

                foreach (Guid accountId in series.Keys)
                    balancesToShow.Add (balances [accountId]);

                // sort the balances descendingly
                balancesToShow.Sort ();

                // now add points for those balances
                int i;
                for (i = 0; i < balancesToShow.Count; i++)
                {
                    AccountBalance balance = balancesToShow[i];

                    double value = 0;
                    if (balance.Balance.HasValue)
                        value = (double)balance.Balance.Value;

                    series[balance.Account.Id].AddPoint (ReportGenerationHelper.GetOrdinalValueForTimescale (firstSegmentStartTime,
                        segmentStartTime, parameters.Timescale),
                        value);

                    if (log.IsDebugEnabled)
                        log.DebugFormat ("Added account '{0}', time '{1}', amount {2}", balance.Account.Name,
                            segmentStartTime.ToShortDateString (), balance.Balance);
                }

                // move to the next segment
                segmentStartTime = segmentEndTime;
            }

            // now add all series to the curvelist (only if they actually contain any points!)
            CurveList curveList = new CurveList ();

            foreach (BarItem item in series.Values)
            {
                if (item.Points.Count > 0)
                {
                    // now check that at least one point has a value != 0
                    bool hasValues = false;
                    for (int i = 0; i < item.Points.Count; i++)
                    {
                        if (item.Points[i].Y != 0)
                        {
                            hasValues = true;
                            break;
                        }
                    }

                    if (hasValues)
                    {
                        item.Color = ReportGenerationHelper.GetColor (
                            parameters.FillPalette.FindColorForObject (ReportParameters.ObjectTypeAccount, ((Account)(item.Tag)).Id.ToString ()));
                        item.Bar.Fill = new Fill (item.Color,
                            ReportGenerationHelper.GetAdjustedColor (item.Color.ToArgb (), 0.75), 180f);
                        curveList.Add (item);
                    }
                }
            }

            zedGraph.GraphPane.CurveList = curveList;

            zedGraph.GraphPane.Title.Text = "test";

            zedGraph.GraphPane.XAxis.Type = AxisType.Text;
            zedGraph.GraphPane.XAxis.MajorTic.IsBetweenLabels = true;
            zedGraph.GraphPane.XAxis.Scale.MinorStep = 1;
            zedGraph.GraphPane.XAxis.Scale.MajorStep = 1;
            zedGraph.GraphPane.XAxis.Scale.TextLabels = textLabels.ToArray ();
            zedGraph.GraphPane.XAxis.Title.IsVisible = false;
            zedGraph.GraphPane.XAxis.Scale.FontSpec.Size = 8;

            zedGraph.GraphPane.YAxis.Title.Text = String.Format (System.Globalization.CultureInfo.InvariantCulture,
                "Amount");
            zedGraph.GraphPane.YAxis.Title.FontSpec.Size = 8;
            zedGraph.GraphPane.YAxis.MajorGrid.IsVisible = true;
            zedGraph.GraphPane.YAxis.MajorGrid.Color = Color.Gray;
            zedGraph.GraphPane.YAxis.Scale.FontSpec.Size = 8;
            zedGraph.GraphPane.YAxis.Type = AxisType.Linear;
            zedGraph.GraphPane.YAxis.Scale.Format = "n0";
            zedGraph.GraphPane.YAxis.Scale.MaxGrace = 0.01;

            zedGraph.GraphPane.BarSettings.Type = BarType.Stack;
            //zedGraph.GraphPane.BarSettings.ClusterScaleWidth = 1;

            zedGraph.AxisChange ();

            generatedReport.ImageFileName = Path.Combine (parameters.ReportDirectory, "BalanceBarchart.png");

            using (Image image = zedGraph.GraphPane.GetImage (parameters.ImageWidth, parameters.ImageHeight, 10))
            {
                image.Save (generatedReport.ImageFileName, ImageFormat.Png);

                return generatedReport;
            }
        }

        static readonly private ILog log = LogManager.GetLogger (typeof (BalanceBarchartReportGenerator));
    }
}
