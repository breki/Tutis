using System;
using System.Collections.Generic;
using NHibernate;

namespace NHTutorial.Model
{
    public class TestDataMother
    {
        public IList<User> Users
        {
            get { return users; }
        }

        public IDictionary<string, Account> Accounts
        {
            get { return accounts; }
        }

        public TestDataMother AddUser(string emailAddress)
        {
            User user = new User();
            user.EmailAddress = emailAddress;
            user.Name = "testuser";
            user.PasswordHash = "12345";
            users.Add(user);
            return this;
        }

        public TestDataMother AddAccount(string accountName, AccountType accountType)
        {
            Account account = LastUser.CreateAccount(accountName, accountType);
            LastUser.Accounts.Add(account);
            accounts[accountName] = account;
            return this;
        }

        public TestDataMother AddTransactions(
            int howMany, 
            string fromAccount, 
            string toAccount, 
            decimal amount, 
            Func<int, DateTime> dateFunc)
        {
            for (int i = 0; i < howMany; i++)
            {
                Transaction transaction = LastUser.CreateTransaction(accounts[fromAccount], accounts[toAccount]);
                transaction.Date = dateFunc(i);
                transaction.Amount = amount;
                transactions.Add(transaction);
            }

            return this;
        }

        public void MakeThem(ISessionFactory sessionFactory)
        {
            using (ISession session = sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                foreach (User user in users)
                    session.Save(user);

                foreach (Transaction trans in transactions)
                    session.Save(trans);

                transaction.Commit();
            }
        }

        protected User LastUser
        {
            get { return users[users.Count - 1]; }
        }

        private List<User> users = new List<User>();
        private Dictionary<string, Account> accounts = new Dictionary<string, Account>();
        private List<Transaction> transactions = new List<Transaction>();
    }
}