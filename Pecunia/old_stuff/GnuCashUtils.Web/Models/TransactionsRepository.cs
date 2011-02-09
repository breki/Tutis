using System.Linq;
using GnuCashUtils.Framework;

namespace GnuCashUtils.Web.Models
{
    public class TransactionsRepository : ITransactionsRepository
    {
        public TransactionsRepository(Book book)
        {
            this.book = book;
        }

        public IQueryable<Transaction> GetTransactions()
        {
            return book.Transactions.Values.AsQueryable();
        }

        private readonly Book book;
    }
}