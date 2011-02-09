using System;
using System.Collections.Generic;
using GnuCashUtils.Framework;

namespace GnuCashUtils.Web.Models
{
    public class AccountWithBalanceHistory
    {
        public AccountWithBalanceHistory(Account account, int historyCapacity)
        {
            this.account = account;

            balances = new List<decimal?>(historyCapacity);
            for (int i = 0; i < historyCapacity; i++)
                balances.Add(new decimal?());
        }

        public Account Account
        {
            get { return account; }
        }

        public IList<decimal?> Balances
        {
            get { return balances; }
        }

        private Account account;
        private IList<decimal?> balances;
    }
}