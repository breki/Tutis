using System;
using System.Collections.Generic;

namespace FinanceReport
{
    public class BalanceByTime
    {
        public IEnumerable<KeyValuePair<DateTime, decimal>> Balances
        {
            get { return balances; }
        }

        public void AddAmount(DateTime date, decimal amount)
        {
            DateTime realDate = date.Date;

            if (!balances.ContainsKey(realDate))
                balances[realDate] = 0;

            balances[realDate] = balances[realDate] + amount;
        }

        private SortedList<DateTime, decimal> balances = new SortedList<DateTime, decimal> ();
    }
}