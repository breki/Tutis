using System;
using System.Collections.Generic;

namespace FinanceReport
{
    public class AmountByTime
    {
        public IEnumerable<KeyValuePair<DateTime, decimal>> Amounts
        {
            get { return amounts; }
        }

        public void AddAmount(DateTime date, decimal amount)
        {
            DateTime realDate = date.Date;

            if (!amounts.ContainsKey(realDate))
                amounts[realDate] = 0;

            amounts[realDate] = amounts[realDate] + amount;
        }

        public void SetAmount (DateTime date, decimal amount)
        {
            DateTime realDate = date.Date;
            amounts[realDate] = amount;
        }

        private SortedList<DateTime, decimal> amounts = new SortedList<DateTime, decimal> ();
    }
}