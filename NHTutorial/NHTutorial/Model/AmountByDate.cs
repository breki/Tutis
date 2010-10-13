using System;

namespace NHTutorial.Model
{
    public class AmountByDate
    {
        public AmountByDate()
        {
        }

        public AmountByDate(DateTime date, Decimal amount)
        {
            Date = date;
            Amount = amount;
        }

        public virtual DateTime Date { get; set; }
        public virtual Decimal Amount { get; set; }
    }
}