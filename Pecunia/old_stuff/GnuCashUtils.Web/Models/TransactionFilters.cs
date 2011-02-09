using System;
using System.Linq;
using GnuCashUtils.Framework;

namespace GnuCashUtils.Web.Models
{
    public static class TransactionFilters
    {
        public static IQueryable<Transaction> OnOrAfter(
            this IQueryable<Transaction> transactions,
            DateTime date)
        {
            return from t in transactions
                   where t.DatePosted.Date >= date.Date
                   select t;
        }

        public static IQueryable<Transaction> Before(
            this IQueryable<Transaction> transactions,
            DateTime beforeDate)
        {
            return from t in transactions
                   where t.DatePosted.Date < beforeDate.Date
                   select t;
        }

        public static IQueryable<Transaction> ForAccount(
            this IQueryable<Transaction> transactions, 
            Guid accountId)
        {
            throw new NotImplementedException();
        }
    }
}