using System;
using System.Globalization;

namespace FinanceReport.Financisto
{
    public class Transaction
    {
        public Transaction(TableRow row)
        {
            date = UnixTimeToDateTime ((string)row.Values["datetime"]);
            fromAmount = decimal.Parse ((string)row.Values["from_amount"], CultureInfo.InvariantCulture) / 100;
            toAmount = decimal.Parse ((string)row.Values["to_amount"], CultureInfo.InvariantCulture) / 100;
            isTemplate = int.Parse ((string)row.Values["is_template"], CultureInfo.InvariantCulture) > 0;
            parentId = int.Parse ((string)row.Values["parent_id"], CultureInfo.InvariantCulture);

            fromAccountId = int.Parse ((string)row.Values["from_account_id"]);
            toAccountId = int.Parse ((string)row.Values["to_account_id"]);
        }

        public DateTime Date
        {
            get { return date; }
        }

        public decimal FromAmount
        {
            get { return fromAmount; }
        }

        public decimal ToAmount
        {
            get { return toAmount; }
        }

        public bool IsTemplate
        {
            get { return isTemplate; }
        }

        public int ParentId
        {
            get { return parentId; }
        }

        public int FromAccountId
        {
            get { return fromAccountId; }
        }

        public int ToAccountId
        {
            get { return toAccountId; }
        }

        public static DateTime UnixTimeToDateTime (string text)
        {
            double seconds = double.Parse (text, CultureInfo.InvariantCulture);
            return Epoch.AddMilliseconds (seconds);
        }

        private static readonly DateTime Epoch = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime date;
        private decimal fromAmount;
        private decimal toAmount;
        private bool isTemplate;
        private int parentId;
        private int fromAccountId;
        private int toAccountId;
    }
}