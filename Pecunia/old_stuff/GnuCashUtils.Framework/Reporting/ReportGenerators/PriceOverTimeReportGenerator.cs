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
    public class PriceOverTimeReportGenerator : IReport
    {
        public GeneratedReport Generate (Book book, ReportParameters parameters)
        {
            GeneratedReport generatedReport = new GeneratedReport ();

            ZedGraphControl zedGraph = ReportGenerationHelper.CreateGraph ();

            CurveList curveList = new CurveList ();

            bool moreThanOneCommodity = parameters.CommoditiesCount > 1;

            foreach (Commodity commodity in parameters.EnumerateCommodities ())
            {
                PointPairList pointsList = new PointPairList ();

                IList<Price> prices = book.ListPricesForCommodity (commodity.FullId, parameters.BaseCurrency.FullId);

                if (prices.Count > 0)
                {
                    decimal normalizer = 1;
                    decimal subtractor = 0;

                    if (moreThanOneCommodity)
                    {
                        normalizer = prices[0].Value.Value / 100m;
                        subtractor = 100;
                    }

                    foreach (Price price in prices)
                        pointsList.Add (price.Time.ToOADate (), (double)(price.Value.Value / normalizer - subtractor));

                    LineItem lineItem = new LineItem (String.Format (System.Globalization.CultureInfo.InvariantCulture,
                        "{0}", commodity.FullId),
                        pointsList,
                        ReportGenerationHelper.GetColor (
                        parameters.LinePalette.FindColorForObject (ReportParameters.ObjectTypeCommodity, commodity.FullId)), 
                        SymbolType.None, 2);
                    //lineItem.Line.IsSmooth = true;
                    lineItem.Line.StepType = StepType.ForwardStep;
                    lineItem.Line.Style = System.Drawing.Drawing2D.DashStyle.Solid;

                    curveList.Add (lineItem);
                }
            }

            zedGraph.GraphPane.CurveList = curveList;
            zedGraph.GraphPane.Title.Text = String.Format (System.Globalization.CultureInfo.InvariantCulture,
                "Price Over Time");

            zedGraph.GraphPane.XAxis.Title.Text = "Time";
            zedGraph.GraphPane.XAxis.Type = AxisType.Date;
            //zedGraph.GraphPane.XAxis.Scale.Format = "d";
            //zedGraph.GraphPane.XAxis.Scale.MajorUnit = DateUnit.Month;
            //zedGraph.GraphPane.XAxis.Scale.MinorUnit = DateUnit.Day;

            zedGraph.GraphPane.XAxis.MajorGrid.Color = Color.Gray;
            zedGraph.GraphPane.XAxis.MajorGrid.IsVisible = true;
            zedGraph.GraphPane.XAxis.Scale.FontSpec.Size = 8;
            zedGraph.GraphPane.YAxis.Scale.MaxGrace = 0.01;
            zedGraph.GraphPane.YAxis.Scale.MinGrace = 0.01;
            zedGraph.GraphPane.XAxis.Title.FontSpec.Size = 8;

            zedGraph.GraphPane.YAxis.MajorGrid.Color = Color.Gray;
            zedGraph.GraphPane.YAxis.MajorGrid.IsVisible = true;
            zedGraph.GraphPane.YAxis.Scale.FontSpec.Size = 8;
            zedGraph.GraphPane.YAxis.Scale.Format = "n2";
            zedGraph.GraphPane.YAxis.Scale.MaxGrace = 0.01;
            zedGraph.GraphPane.YAxis.Scale.MinGrace = 0.01;
            zedGraph.GraphPane.YAxis.Title.FontSpec.Size = 8;
            zedGraph.GraphPane.YAxis.Title.Text = String.Format (System.Globalization.CultureInfo.InvariantCulture,
                "Price {0}", moreThanOneCommodity ? "(%)" : null);
            zedGraph.GraphPane.YAxis.Type = AxisType.Linear;
            zedGraph.GraphPane.YAxis.Color = Color.LightGray;

            zedGraph.AxisChange ();

            generatedReport.ImageFileName = Path.Combine (parameters.ReportDirectory, "PriceOverTime.png");

            using (Image image = zedGraph.GraphPane.GetImage (parameters.ImageWidth, parameters.ImageHeight, 10))
            {
                image.Save (generatedReport.ImageFileName, ImageFormat.Png);

                return generatedReport;
            }
        }
    }
}
