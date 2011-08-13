using System;
using System.Collections.Generic;
using GnuCashUtils.Framework;

namespace GnuCashUtils.Web.Models
{
    public class BalancesCalculator : IBalancesCalculator
    {
        public BalancesCalculator(Book book)
        {
            this.book = book;
        }

        public IDictionary<Guid, AccountWithBalanceHistory> CalculateAccountsBalancesForTheseDates(IList<DateTime> dates)
        {
            Dictionary<Guid, AccountWithBalanceHistory> accountsWithBalances = new Dictionary<Guid, AccountWithBalanceHistory>();

            foreach (Transaction transaction in book.Transactions.Values)
                foreach (TransactionSplit split in transaction.Splits)
                    EnterTransactionSplitIntoBalances(split, split.Account, accountsWithBalances, dates);

            return accountsWithBalances;
        }

        private void EnterTransactionSplitIntoBalances(
            TransactionSplit split, 
            Account account,
            IDictionary<Guid, AccountWithBalanceHistory> balanceHistories, 
            IList<DateTime> dates)
        {
            if (false == balanceHistories.ContainsKey(account.Id))
                balanceHistories[account.Id] = new AccountWithBalanceHistory(account, dates.Count);

            for (int i = 0; i < dates.Count; i++)
            {
                if (split.Transaction.DatePosted <= dates[i])
                {
                    if (balanceHistories[account.Id].Balances[i].HasValue)
                        balanceHistories[account.Id].Balances[i] += split.Value.Value;
                    else
                        balanceHistories[account.Id].Balances[i] = split.Value.Value;
                }
            }

            if (account.ParentAccount != null)
                EnterTransactionSplitIntoBalances(split, account.ParentAccount, balanceHistories, dates);
        }

        private readonly Book book;
    }
}