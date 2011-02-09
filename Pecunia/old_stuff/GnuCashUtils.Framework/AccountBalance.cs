using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GnuCashUtils.Framework
{
    [SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes")]
    public class AccountBalance : IComparable<AccountBalance>
    {
        public Account Account
        {
            get { return account; }
        }

        public decimal? Balance
        {
            get { return balance; }
            set { balance = value; }
        }

        public AccountBalance (Account account)
        {
            this.account = account;
        }

        public int CompareTo (AccountBalance other)
        {
            if (this == other)
                return 0;
            if (other == null)
                return 1;

            int c;
            if (this.balance.HasValue && other.balance.HasValue)
                c = -decimal.Compare (this.balance.Value, other.balance.Value);
            else if (this.balance.HasValue)
                c = 1;
            else if (other.balance.HasValue)
                c = -1;
            else
                c = 0;

            if (c != 0)
                return c;

            return String.Compare (this.account.Name, other.account.Name);
        }

        private Account account;
        private decimal? balance;
    }
}
