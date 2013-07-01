using System.Collections.Generic;

namespace FinanceReport.DataModel
{
    public class CategoriesRangesAmounts
    {
        public void AddAmount (int groupId, string groupName, int monthIndex, decimal amount)
        {
            if (!categoryNames.ContainsKey(groupId))
            {
                categoryNames.Add(groupId, groupName);
                amounts.Add(groupId, new AmountByRange());
            }

            amounts[groupId].AddAmount(monthIndex, amount);
        }

        public SortedList<int, string> CategoryNames
        {
            get { return categoryNames; }
        }

        public SortedList<int, AmountByRange> Amounts
        {
            get { return amounts; }
        }

        public AmountByRange GetAmountsForCategory(int category)
        {
            return amounts[category];
        }

        private SortedList<int, string> categoryNames = new SortedList<int, string>();
        private SortedList<int, AmountByRange> amounts = new SortedList<int, AmountByRange>();
    }
}