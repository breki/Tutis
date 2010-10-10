using System.Collections.Generic;
using MbUnit.Framework;
using NHibernate;
using NHTutorial.Model;
using NHTutorial.Repositories;

namespace NHTutorial.Tests
{
    public class AccountRepositoryTests
    {
        [Test]
        public void AddAccounts()
        {
            User user;
            using (ISession session = sessionFactory.OpenSession())
            {
                using(ITransaction transaction = session.BeginTransaction())
                {
                    user = new User();
                    user.EmailAddress = EmailAddress;
                    user.Name = "Billy";
                    user.PasswordHash = "sdsdsds";

                    usersRepository.CreateUser(session, user);
                    Account account = user.CreateAccount("income", AccountType.Income);
                    accountsRepository.AddAccount(session, account);
                    account = user.CreateAccount("expenses", AccountType.Expense);
                    accountsRepository.AddAccount(session, account);

                    transaction.Commit();
                }
            }

            using (ISession session = sessionFactory.OpenSession())
            {
                IList<Account> accounts = accountsRepository.ListUsersAccounts(session, user.Id);
                Assert.AreEqual(2, accounts.Count);
            }
        }

        [SetUp]
        private void Setup()
        {
            sessionFactory = DBTestsHelper.CreateSessionFactory();
            usersRepository = new UsersRepository();
            accountsRepository = new AccountsRepository();

            using (ISession session = sessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    usersRepository.DeleteUser(session, EmailAddress);
                    transaction.Commit();
                }
            }
        }

        private const string EmailAddress = "bill.gates@microsoft.com";
        private ISessionFactory sessionFactory;
        private IUsersRepository usersRepository;
        private IAccountsRepository accountsRepository;
    }
}