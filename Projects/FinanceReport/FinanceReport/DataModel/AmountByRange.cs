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

        public AmountByRange CalculateTrend (int averages)
        {
            AmountByRange trend = new AmountByRange();
            for (int i = 0; i < amounts.Count; i++)
            {
                int key = amounts.Keys[i];
                decimal trendValue = 0;
                int trendValuesCount = 0;

                for (int j = 0; j < averages; j++)
                {
                    int trendKey = key - j;
                    if (trendKey < 0 || !amounts.ContainsKey(trendKey))
                        continue;

                    trendValue += amounts[trendKey];
                    trendValuesCount++;
                }

                if (trendValuesCount > 0)
                    trend.SetAmount(key, trendValue / trendValuesCount);
            }

            return trend;
        }

        private SortedList<int, decimal> amounts = new SortedList<int, decimal> ();
    }
}