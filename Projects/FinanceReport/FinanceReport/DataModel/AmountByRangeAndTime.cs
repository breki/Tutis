using System;
using System.Collections.Generic;

namespace FinanceReport.DataModel
{
    public class AmountByRangeAndTime
    {
        public IEnumerable<KeyValuePair<int, AmountByTime>> Amounts
        {
            get { return amounts; }
        }

        public void AddAmount (int index, DateTime date, decimal amount)
        {
            if (!amounts.ContainsKey (index))
                amounts[index] = new AmountByTime();

            amounts[index].AddAmount(date, amount);
        }

        public void SetAmount (int index, DateTime date, decimal amount)
        {
            if (!amounts.ContainsKey (index))
                amounts[index] = new AmountByTime ();

            amounts[index].SetAmount(date, amount);
        }

        public AmountByTime GetByIndex(int index)
        {
            return amounts[index];
        }

        private SortedList<int, AmountByTime> amounts = new SortedList<int, AmountByTime> ();
    }
}