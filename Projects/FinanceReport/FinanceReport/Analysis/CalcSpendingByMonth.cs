using System;
using FinanceReport.DataModel;

namespace FinanceReport.Analysis
{
    public class CalcSpendingByMonth : CalcBase
    {
        public static AmountByRange Calc(Database db)
        {
            DatabaseTable table = db.Tables["transactions"];

            AmountByRange data = new AmountByRange ();

            foreach (TableRow row in table.Rows.Values)
            {
                Transaction tx = new Transaction (row);

                if (tx.IsExpenseTransaction)
                    data.AddAmount (GetMonthIndex (tx.Date) - GetMonthIndex (new DateTime (2011, 1, 1)), tx.FromAmount);
            }

            return data;
        }
    }
}