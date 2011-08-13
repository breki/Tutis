using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using GnuCashUtils.Framework.Reporting;
using GnuCashUtils.Framework;
using GnuCashUtils.Framework.Reporting.ReportGenerators;

namespace GnuCashUtils.Test.ReportingTests
{
    [TestFixture]
    public class AssetsPiechartTests
    {
        [MbUnit.Framework.Test]
        public void Assets ()
        {
            ReportParameters parameters = new ReportParameters ();
            parameters.BaseCurrency = euroCommodity;

            AssetsPiechartReportGenerator report = new AssetsPiechartReportGenerator ();
            report.Generate (book, parameters);
        }

        [MbUnit.Framework.Test]
        public void Expenses ()
        {
            ReportParameters parameters = new ReportParameters ();
            parameters.BaseCurrency = euroCommodity;
            parameters.StartTime = DateTime.Now.AddMonths (-1);

            ExpensePiechartReportGenerator report = new ExpensePiechartReportGenerator ();
            report.Generate (book, parameters);
        }

        [MbUnit.Framework.Test]
        public void AssetsOverTime ()
        {
            ReportParameters parameters = new ReportParameters ();
            parameters.BaseCurrency = euroCommodity;
            parameters.StartTime = new DateTime (2007, 06, 06);
            parameters.IncludeAccount (book.GetAccountById (new Guid ("e4da11735697ee6ada9ce30595fa772e")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("3692191fb280f91212784c903284e8f2")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("77f71fd06e41d5868d36b05aa9d581e5")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("e0d10b0b8ff4b721a23206ba4d1b6ded")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("55c34093efddd5cb3825189620cfdc30")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("b1c27b8d340d29db007a5f843e27132f")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("383ac952b939031f7cd07dc186141c92")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("db6a1af7ac7937ff32f093cfa26d817f")));

            AssetsOverTimeReportGenerator report = new AssetsOverTimeReportGenerator ();
            report.Generate (book, parameters);
        }

        [MbUnit.Framework.Test]
        public void NetWorthOverTime ()
        {
            ReportParameters parameters = new ReportParameters ();
            parameters.BaseCurrency = euroCommodity;
            parameters.StartTime = new DateTime (2007, 06, 06);
            parameters.IncludeAccount (book.GetAccountById (new Guid ("e4da11735697ee6ada9ce30595fa772e")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("3692191fb280f91212784c903284e8f2")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("77f71fd06e41d5868d36b05aa9d581e5")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("e0d10b0b8ff4b721a23206ba4d1b6ded")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("55c34093efddd5cb3825189620cfdc30")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("b1c27b8d340d29db007a5f843e27132f")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("383ac952b939031f7cd07dc186141c92")));
            parameters.IncludeAccount (book.GetAccountById (new Guid ("db6a1af7ac7937ff32f093cfa26d817f")));

            NetWorthOverTimeReportGenerator report = new NetWorthOverTimeReportGenerator ();
            report.Generate (book, parameters);
        }

        [MbUnit.Framework.Test]
        public void BalanceBarchart ()
        {
            ReportParameters parameters = new ReportParameters ();
            parameters.BaseCurrency = euroCommodity;
            parameters.StartTime = new DateTime (2007, 04, 01);
            parameters.EndTime = DateTime.Now;
            parameters.Timescale = ReportTimescale.Monthly;

            Account rootAccount = book.FindRootAccountForType (AccountType.Expense);
            foreach (Account account in rootAccount.ChildAccounts)
            {
                if (false == account.IsPlaceholder && account.Type == AccountType.Expense)
                    parameters.IncludeAccount (account);
            }

            BalanceBarchartReportGenerator report = new BalanceBarchartReportGenerator ();
            report.Generate (book, parameters);
        }

        [SetUp]
        public void Setup ()
        {
            book = new XmlBookReader(@"..\..\..\Data\Igor2.xml").Read();
            euroCommodity = book.GetCommodity (Commodity.ConstructFullId ("ISO4217", "EUR"));
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup ()
        {
            log4net.Config.XmlConfigurator.Configure ();
        }

        private Book book;
        private Commodity euroCommodity;
    }
}
