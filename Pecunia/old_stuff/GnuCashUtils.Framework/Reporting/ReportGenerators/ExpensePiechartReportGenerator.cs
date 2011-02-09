using System;
using System.Collections.Generic;
using System.Text;

namespace GnuCashUtils.Framework.Reporting.ReportGenerators
{
    public class ExpensePiechartReportGenerator : BalancePiechartReportGenerator, IReport
    {
        public GeneratedReport Generate (Book book, ReportParameters parameters)
        {
            string title = String.Format (System.Globalization.CultureInfo.InvariantCulture,
                "Expenses\n{0}", ReportGenerationHelper.ConstructTimeframeString (parameters));
            return Generate (book, parameters, title, "ExpensesPiechart.png", AccountType.Expense, parameters.StartTime, parameters.EndTime);
        }
    }
}
