using System;
using System.Linq;
using GnuCashUtils.Framework;

namespace GnuCashUtils.Web.Models
{
    public interface ITransactionsRepository
    {
        IQueryable<Transaction> GetTransactions();
    }
}