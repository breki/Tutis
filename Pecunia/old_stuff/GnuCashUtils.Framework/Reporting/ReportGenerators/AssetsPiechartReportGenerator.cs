using System;

namespace GnuCashUtils.Framework.Reporting.ReportGenerators
{
    public class AssetsPiechartReportGenerator : BalancePiechartReportGenerator, IReport
    {
        public GeneratedReport Generate (Book book, ReportParameters parameters)
        {
            return Generate (book, parameters, "Assets", "AssetsPiechart.png", AccountType.Asset, null, DateTime.Now);
        }
    }
}
