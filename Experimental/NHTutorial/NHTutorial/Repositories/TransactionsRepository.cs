using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHTutorial.Model;

namespace NHTutorial.Repositories
{
    public interface ITransactionsRepository
    {
        void AddTransaction(ISession session, Transaction transaction);

        IList<Transaction> ListTransactionsWithinDates(
            ISession session,
            int userId,
            DateTime fromDate,
            DateTime tillDate);

        IList<AmountByDate> ListAccountBalancesByDate(
            ISession session,
            int userId,
            int accountId,
            DateTime fromDate,
            DateTime tillDate);

        IList<Transaction> ListTransactionsFromAccount (
            ISession session,
            int userId,
            int accountId,
            DateTime fromDate,
            DateTime tillDate);
    }

    public class TransactionsRepository : ITransactionsRepository
    {
        public void AddTransaction(ISession session, Transaction transaction)
        {
            session.Save(transaction);
        }

        public IList<Transaction> ListTransactionsWithinDates(
            ISession session,
            int userId,
            DateTime fromDate,
            DateTime tillDate)
        {
            return session.CreateCriteria<Transaction>()
                .Add(Expression.Between("Date", fromDate, tillDate))
                .SetMaxResults(1000)
                .AddOrder(Order.Asc("Date"))
                .AddOrder(Order.Asc("Id"))
                .List<Transaction>();
        }

        public IList<AmountByDate> ListAccountBalancesByDate(
            ISession session,
            int userId,
            int accountId,
            DateTime fromDate,
            DateTime tillDate)
        {
            return session.CreateQuery(
                @"select new AmountByDate (t.Date, t.Amount)
 from Transaction t
 where t.Date >= :fromDate
 and t.Date < :tillDate")
                .SetDateTime("fromDate", fromDate)
                .SetDateTime("tillDate", tillDate)
                .List<AmountByDate>();
        }

        public IList<Transaction> ListTransactionsFromAccount (
            ISession session,
            int userId,
            int accountId,
            DateTime fromDate,
            DateTime tillDate)
        {
            Account fromAccount = session.Get<Account>(accountId);
            if (fromAccount.User.Id != userId)
                throw new ArgumentException("UserId and AccountId do not match");

            return session.CreateCriteria<Transaction>()
                .Add(Expression.Between("Date", fromDate, tillDate))
                .Add(Expression.Eq("FromAccount", fromAccount))
                .SetMaxResults(1000)
                .AddOrder(Order.Asc("Date"))
                .AddOrder(Order.Asc("Id"))
                .List<Transaction>();            
        }
    }
}