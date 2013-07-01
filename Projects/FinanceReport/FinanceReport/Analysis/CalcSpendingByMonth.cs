using System;
using FinanceReport.DataModel;

namespace FinanceReport.Analysis
{
    public class CalcSpendingByMonth : CalcBase
    {
        public static AmountByRange Calc(Database db)
        {
            DateTime now = DateTime.Now;
            int nowMonthIndex = GetMonthIndex (now) - GetMonthIndex (new DateTime (2011, 1, 1));

            DatabaseTable table = db.Tables["transactions"];

            AmountByRange data = new AmountByRange ();

            foreach (TableRow row in table.Rows.Values)
            {
                Transaction tx = new Transaction (row);

                if (tx.IsExpenseTransaction)
                {
                    int monthIndex = GetMonthIndex(tx.Date) - GetMonthIndex(new DateTime(2011, 1, 1));
                    if (nowMonthIndex - monthIndex > 13)
                        continue;

                    data.AddAmount (monthIndex, tx.FromAmount);
                }
            }

            return data;
        }
    }
}