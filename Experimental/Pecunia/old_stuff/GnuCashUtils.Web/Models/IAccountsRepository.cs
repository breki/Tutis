using System;
using System.Linq;
using GnuCashUtils.Framework;

namespace GnuCashUtils.Web.Models
{
    public interface IAccountsRepository
    {
        Account RootAccount { get; }

        IQueryable<Account> GetAccounts();
    }
}