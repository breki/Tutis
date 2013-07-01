using System;
using FinanceReport.DataModel;

namespace FinanceReport.Analysis
{
    public class CalcEarningByMonth : CalcBase
    {
        public static AmountByRange Calc(Database db)
        {
            DatabaseTable table = db.Tables["transactions"];

            AmountByRange data = new AmountByRange ();

            foreach (TableRow row in table.Rows.Values)
            {
                Transaction tx = new Transaction (row);

                if (tx.IsTemplate || tx.ParentId > 0)
                    continue;

                if (tx.ToAccountId == 0 && tx.FromAmount >= 0)
                    data.AddAmount (
                        GetMonthIndex (tx.Date) - GetMonthIndex (new DateTime (2011, 1, 1)), tx.FromAmount);
            }

            return data;
        }
    }
}