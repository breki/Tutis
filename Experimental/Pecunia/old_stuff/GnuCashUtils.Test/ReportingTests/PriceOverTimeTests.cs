using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using GnuCashUtils.Framework;
using GnuCashUtils.Framework.Reporting.ReportGenerators;
using GnuCashUtils.Framework.Reporting;
using System.Drawing;

namespace GnuCashUtils.Test.ReportingTests
{
    [TestFixture]
    public class PriceOverTimeTests
    {
        [MbUnit.Framework.Test]
        public void TestSingleCommodity ()
        {
            ReportParameters parameters = new ReportParameters ();
            parameters.BaseCurrency = euroCommodity;
            parameters.IncludeCommodity (book.GetCommodity (Commodity.ConstructFullId ("FUND", "KDBAL")));

            parameters.LinePalette.AssignColor (Commodity.ConstructFullId ("FUND", "KDBAL"), Color.DarkGreen.ToArgb());
            parameters.LinePalette.AssignColor (Commodity.ConstructFullId ("FUND", "KMG"), Color.DarkRed.ToArgb ());
            parameters.LinePalette.AssignColor (Commodity.ConstructFullId ("FUND", "PRA"), Color.DarkSlateBlue.ToArgb ());

            PriceOverTimeReportGenerator report = new PriceOverTimeReportGenerator ();
            report.Generate (book, parameters);
        }

        [MbUnit.Framework.Test]
        public void TestMultipleCommodities ()
        {
            ReportParameters parameters = new ReportParameters ();
            parameters.BaseCurrency = euroCommodity;
            parameters.IncludeCommodity (book.GetCommodity (Commodity.ConstructFullId ("FUND", "KDBAL")));
            parameters.IncludeCommodity (book.GetCommodity (Commodity.ConstructFullId ("FUND", "KMG")));
            parameters.IncludeCommodity (book.GetCommodity (Commodity.ConstructFullId ("FUND", "PRA")));
            parameters.IncludeCommodity (book.GetCommodity (Commodity.ConstructFullId ("ISO4217", "HRK")));
            parameters.IncludeCommodity (book.GetCommodity (Commodity.ConstructFullId ("ISO4217", "USD")));

            parameters.LinePalette.AssignColor (Commodity.ConstructFullId ("FUND", "KDBAL"), Color.DarkGreen.ToArgb ());
            parameters.LinePalette.AssignColor (Commodity.ConstructFullId ("FUND", "KMG"), Color.DarkRed.ToArgb ());
            parameters.LinePalette.AssignColor (Commodity.ConstructFullId ("FUND", "PRA"), Color.DarkSlateBlue.ToArgb ());

            PriceOverTimeReportGenerator report = new PriceOverTimeReportGenerator ();
            report.Generate (book, parameters);
        }

        [SetUp]
        public void Setup ()
        {
            book = new XmlBookReader(@"..\..\..\Data\Igor.xml").Read();
            euroCommodity = book.GetCommodity(Commodity.ConstructFullId("ISO4217", "EUR"));
        }

        private Book book;
        private Commodity euroCommodity;
    }
}
