using System;
using System.Linq;
using GnuCashUtils.Framework;

namespace GnuCashUtils.Web.Models
{
    public static class AccountFilters
    {
        public static Account ById (
            this IQueryable<Account> accounts,
            Guid accountId)
        {
            return accounts.First(a => a.Id == accountId);
        }        
    }
}