using FinanceReport.DataModel;

namespace FinanceReport.Reporting.Models
{
    public class MainReportModel
    {
        public AmountByTime BalancesDailyDiffs
        {
            get { return balancesDailyDiffs; }
            set { balancesDailyDiffs = value; }
        }

        public AmountByRange SpendingByMonth
        {
            get { return spendingByMonth; }
            set { spendingByMonth = value; }
        }

        public AmountByRange EarningByMonth
        {
            get { return earningByMonth; }
            set { earningByMonth = value; }
        }

        public AmountByTime BalancesDaily
        {
            get { return balancesDaily; }
            set { balancesDaily = value; }
        }

        public CategoriesRangesAmounts MonthlySpendingByCategories
        {
            get { return monthlySpendingByCategories; }
            set { monthlySpendingByCategories = value; }
        }

        private AmountByTime balancesDailyDiffs;
        private AmountByRange spendingByMonth;
        private AmountByRange earningByMonth;
        private AmountByTime balancesDaily;
        private CategoriesRangesAmounts monthlySpendingByCategories;
    }
}