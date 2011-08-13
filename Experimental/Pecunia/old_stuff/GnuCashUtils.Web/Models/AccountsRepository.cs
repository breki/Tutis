using System;
using System.Linq;
using GnuCashUtils.Framework;

namespace GnuCashUtils.Web.Models
{
    public class AccountsRepository : IAccountsRepository
    {
        public AccountsRepository(Book book)
        {
            this.book = book;
        }

        public Account RootAccount
        {
            get { return book.RootAccount; }
        }

        public IQueryable<Account> GetAccounts()
        {
            return book.EnumerateAccounts().AsQueryable();
        }

        private readonly Book book;
    }
}