using System.Collections.Generic;

namespace FinanceReport.DataModel
{
    public class AmountByRange
    {
        public IEnumerable<KeyValuePair<int, decimal>> Amounts
        {
            get { return amounts; }
        }

        public void AddAmount (int index, decimal amount)
        {
            if (!amounts.ContainsKey (index))
                amounts[index] = 0;

            amounts[index] = amounts[index] + amount;
        }

        public void SetAmount (int index, decimal amount)
        {
            amounts[index] = amount;
        }

        private SortedList<int, decimal> amounts = new SortedList<int, decimal> ();
    }
}