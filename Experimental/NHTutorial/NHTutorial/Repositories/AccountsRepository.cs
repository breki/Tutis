using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHTutorial.Model;

namespace NHTutorial.Repositories
{
    public interface IAccountsRepository
    {
        void AddAccount(ISession session, Account account);
        IList<Account> ListUsersAccounts(ISession session, int userId);
    }

    public class AccountsRepository : IAccountsRepository
    {
        public void AddAccount(ISession session, Account account)
        {
            session.Save(account);
        }

        public IList<Account> ListUsersAccounts(ISession session, int userId)
        {
            ICriteria criteria = session.CreateCriteria<Account>();
            criteria.Add(Expression.Eq("User.Id", userId));
            return criteria.List<Account>();
        }
    }
}