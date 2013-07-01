using FinanceReport.DataModel;

namespace FinanceReport.Analysis
{
    public class CalcBalancesDiffs : CalcBase
    {
        public static AmountByTime Calc (Database db)
        {
            AmountByTime balances = new AmountByTime ();

            DatabaseTable table = db.Tables["transactions"];
            foreach (TableRow row in table.Rows.Values)
            {
                Transaction tx = new Transaction (row);

                if (tx.IsTemplate || tx.ParentId > 0)
                    continue;

                if (tx.ToAccountId == 0)
                    balances.AddAmount (tx.Date, tx.FromAmount);
            }

            return balances;
        }
    }
}