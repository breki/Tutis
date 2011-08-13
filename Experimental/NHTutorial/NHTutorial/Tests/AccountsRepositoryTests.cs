using System.Collections.Generic;
using MbUnit.Framework;
using NHibernate;
using NHTutorial.Model;

namespace NHTutorial.Tests
{
    public class AccountsRepositoryTests : DBTestsBase
    {
        [Test]
        public void AddAccounts()
        {
            User user;
            using (ISession session = SessionFactory.OpenSession())
            {
                using(ITransaction transaction = session.BeginTransaction())
                {
                    user = new User();
                    user.EmailAddress = EmailAddress;
                    user.Name = "Billy";
                    user.PasswordHash = "sdsdsds";

                    UsersRepository.CreateUser(session, user);
                    Account account = user.CreateAccount("income", AccountType.Income);
                    AccountsRepository.AddAccount(session, account);
                    account = user.CreateAccount("expenses", AccountType.Expense);
                    AccountsRepository.AddAccount(session, account);

                    transaction.Commit();
                }
            }

            using (ISession session = SessionFactory.OpenSession())
            {
                IList<Account> accounts = AccountsRepository.ListUsersAccounts(session, user.Id);
                Assert.AreEqual(2, accounts.Count);
            }
        }

        [Test]
        public void AddAccounts2()
        {
            User user;
            using (ISession session = SessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    user = new User();
                    user.EmailAddress = EmailAddress;
                    user.Name = "Billy";
                    user.PasswordHash = "sdsdsds";

                    Account account = user.CreateAccount("income", AccountType.Income);
                    user.Accounts.Add(account);
                    account = user.CreateAccount("expenses", AccountType.Expense);
                    user.Accounts.Add(account);

                    UsersRepository.CreateUser(session, user);

                    transaction.Commit();
                }
            }

            using (ISession session = SessionFactory.OpenSession())
            {
                IList<Account> accounts = AccountsRepository.ListUsersAccounts(session, user.Id);
                Assert.AreEqual(2, accounts.Count);
            }
        }
    }
}