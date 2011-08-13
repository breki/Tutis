using System;
using System.Collections.Generic;
using System.Text;
using GnuCashUtils.Framework;
using System.IO;
using GnuCashUtils.Framework.Reporting;
using GnuCashUtils.Framework.Reporting.ReportGenerators;
using System.Xml;

namespace GnuCashUtils.Console
{
    public class DailyReportCommand : ICommand
    {
        public string ReportsRootDirectory
        {
            get { return reportsRootDirectory; }
            set { reportsRootDirectory = value; }
        }

        #region ICommand Members

        public void Execute (string gnuCashFileName)
        {
            Book book = new XmlBookReader(gnuCashFileName).Read();

            Commodity euroCommodity = book.GetCommodity (Commodity.ConstructFullId ("ISO4217", "EUR"));

            if (false == Directory.Exists (reportsRootDirectory))
                Directory.CreateDirectory (reportsRootDirectory);

            string dailyReportDir = Path.Combine (reportsRootDirectory, "DailyReport");
            if (Directory.Exists (dailyReportDir))
                Directory.Delete (dailyReportDir, true);

            Directory.CreateDirectory (dailyReportDir);

            // copy XSLT files
            File.Copy (@"Xslt\DailyReport.xslt", Path.Combine (dailyReportDir, "DailyReport.xslt"));

            ReportParameters parameters;
            IReport report;

            parameters = PrepareParameters (dailyReportDir, euroCommodity);
            parameters.StartTime = DateTime.Now.AddMonths (-12);
            parameters.IncludeAccount (book.GetAccountById (new Guid ("e4da11735697ee6ada9ce30595fa772e")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("3692191fb280f91212784c903284e8f2")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("77f71fd06e41d5868d36b05aa9d581e5")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("e0d10b0b8ff4b721a23206ba4d1b6ded")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("55c34093efddd5cb3825189620cfdc30")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("b1c27b8d340d29db007a5f843e27132f")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("383ac952b939031f7cd07dc186141c92")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("db6a1af7ac7937ff32f093cfa26d817f")));

            report = new AssetsOverTimeReportGenerator ();
            report.Generate (book, parameters);

            parameters = PrepareParameters (dailyReportDir, euroCommodity);
            report = new AssetsPiechartReportGenerator ();
            report.Generate (book, parameters);

            parameters = PrepareParameters (dailyReportDir, euroCommodity);
            parameters.StartTime = DateTime.Now.AddMonths (-12);
            parameters.Timescale = ReportTimescale.Monthly;
            report = new BalanceBarchartReportGenerator ();
            report.Generate (book, parameters);

            parameters = PrepareParameters (dailyReportDir, euroCommodity);
            parameters.StartTime = DateTime.Now.AddMonths (-12);
            report = new ExpensePiechartReportGenerator ();
            report.Generate (book, parameters);

            parameters = PrepareParameters (dailyReportDir, euroCommodity);
            parameters.StartTime = DateTime.Now.AddMonths (-12);
            parameters.IncludeAccount (book.GetAccountById (new Guid ("e4da11735697ee6ada9ce30595fa772e")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("3692191fb280f91212784c903284e8f2")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("77f71fd06e41d5868d36b05aa9d581e5")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("e0d10b0b8ff4b721a23206ba4d1b6ded")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("55c34093efddd5cb3825189620cfdc30")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("b1c27b8d340d29db007a5f843e27132f")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("383ac952b939031f7cd07dc186141c92")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("db6a1af7ac7937ff32f093cfa26d817f")));

            report = new NetWorthOverTimeReportGenerator ();
            report.Generate (book, parameters);

            parameters = PrepareParameters (dailyReportDir, euroCommodity);
            parameters.StartTime = DateTime.Now.AddMonths (-12);
            parameters.IncludeCommodity (book.GetCommodity (Commodity.ConstructFullId ("FUND", "KDBAL")));
            parameters.IncludeCommodity (book.GetCommodity (Commodity.ConstructFullId ("FUND", "KMG")));
            parameters.IncludeCommodity (book.GetCommodity (Commodity.ConstructFullId ("FUND", "PRA")));
            parameters.IncludeCommodity (book.GetCommodity (Commodity.ConstructFullId ("ISO4217", "HRK")));
            parameters.IncludeCommodity (book.GetCommodity (Commodity.ConstructFullId ("ISO4217", "USD")));

            report = new PriceOverTimeReportGenerator ();
            report.Generate (book, parameters);

            using (Stream stream = File.Open (Path.Combine (dailyReportDir, "DailyReport.xml"), FileMode.Create, FileAccess.Write))
            {
                using (XmlTextWriter writer = new XmlTextWriter (stream, Encoding.Unicode))
                {
                    writer.WriteStartDocument ();
                    writer.WriteProcessingInstruction ("xml-stylesheet", "type=\"text/xsl\" href=\"DailyReport.xslt\"");

                    writer.WriteStartElement ("Reports");

                    writer.WriteAttributeString ("Title", "Daily Report");
                    writer.WriteAttributeString ("GenerationTime", DateTime.Now.ToString ("F", System.Globalization.CultureInfo.InvariantCulture));

                    writer.WriteStartElement ("Report");
                    writer.WriteElementString ("Title", "Assets Over Time");
                    writer.WriteElementString ("Image", "AssetsOverTime.png");
                    writer.WriteEndElement ();

                    writer.WriteStartElement ("Report");
                    writer.WriteElementString ("Title", "Assets Piechart");
                    writer.WriteElementString ("Image", "AssetsPiechart.png");
                    writer.WriteEndElement();

                    writer.WriteStartElement ("Report");
                    writer.WriteElementString ("Title", "Balance Barchart");
                    writer.WriteElementString ("Image", "BalanceBarchart.png");
                    writer.WriteEndElement ();

                    writer.WriteStartElement ("Report");
                    writer.WriteElementString ("Title", "Expenses Piechart");
                    writer.WriteElementString ("Image", "ExpensesPiechart.png");
                    writer.WriteEndElement ();

                    writer.WriteStartElement ("Report");
                    writer.WriteElementString ("Title", "Net Worth Over Time");
                    writer.WriteElementString ("Image", "NetWorthOverTime.png");
                    writer.WriteEndElement ();

                    writer.WriteStartElement ("Report");
                    writer.WriteElementString ("Title", "Prices Over Time");
                    writer.WriteElementString ("Image", "PriceOverTime.png");
                    writer.WriteEndElement ();

                    writer.WriteEndElement ();

                    writer.WriteEndDocument ();
                }
            }
        }

        private ReportParameters PrepareParameters (string reportDirectory, Commodity commodity)
        {
            ReportParameters parameters = new ReportParameters ();
            parameters.BaseCurrency = commodity;
            parameters.ReportDirectory = reportDirectory;

            return parameters;
        }

        #endregion

        private string reportsRootDirectory;
    }
}
