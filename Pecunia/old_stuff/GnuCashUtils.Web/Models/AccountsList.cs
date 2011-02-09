using System;
using System.Collections.Generic;
using GnuCashUtils.Framework;

namespace GnuCashUtils.Web.Models
{
    public class AccountsList
    {
        public AccountsList(Account rootAccount, IDictionary<Guid, AccountWithBalanceHistory> accountsWithBalances)
        {
            this.rootAccount = rootAccount;
            this.accountsWithBalances = accountsWithBalances;
        }

        public Account RootAccount
        {
            get { return rootAccount; }
        }

        public IDictionary<Guid, AccountWithBalanceHistory> AccountsWithBalances
        {
            get { return accountsWithBalances; }
        }

        private Account rootAccount;
        private IDictionary<Guid, AccountWithBalanceHistory> accountsWithBalances;
    }
}