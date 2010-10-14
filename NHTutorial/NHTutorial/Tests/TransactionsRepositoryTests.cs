using System;
using System.Collections.Generic;
using MbUnit.Framework;
using NHibernate;
using NHTutorial.Model;

namespace NHTutorial.Tests
{
    public class TransactionsRepositoryTests : DBTestsBase
    {
        [Test]
        public void ListTransactionsByDate()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction trans = session.BeginTransaction())
            {
                IList<Transaction> transactions = TransactionsRepository.ListTransactionsWithinDates(
                    session,
                    mother.Users[0].Id,
                    new DateTime(2010, 10, 1),
                    new DateTime(2010, 11, 1));

                Assert.AreEqual(11, transactions.Count);
            }
        }

        [Test, Pending("Not working, see http://support.fluentnhibernate.org/discussions/help/343-importtype-not-working")]
        public void ListAmountsByDate()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction trans = session.BeginTransaction())
            {
                IList<AmountByDate> amounts = TransactionsRepository.ListAccountBalancesByDate(
                    session,
                    mother.Users[0].Id,
                    mother.Accounts["account1"].Id,
                    new DateTime(2010, 10, 1),
                    new DateTime(2010, 11, 1));
                Assert.AreEqual(10, amounts.Count);
            }
        }

        [Test]
        public void ListTransactionsFromAccount()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction trans = session.BeginTransaction())
            {
                IList<Transaction> transactions = TransactionsRepository.ListTransactionsFromAccount(
                    session,
                    mother.Users[0].Id,
                    mother.Accounts["account1"].Id,
                    new DateTime(2010, 10, 1),
                    new DateTime(2010, 11, 1));

                Assert.AreEqual(10, transactions.Count);
            }
        }

        protected override void Setup()
        {
            base.Setup();

            mother = new TestDataMother();
            mother
                .AddUser(EmailAddress)
                .AddAccount("account1", AccountType.Income)
                .AddAccount("account2", AccountType.Expense)
                .AddTransactions(10, "account1", "account2", 100, i => new DateTime(2010, 10, 14).AddDays(1))
                .AddTransactions(1, "account2", "account1", 100, i => new DateTime(2010, 10, 14))
                .MakeThem(SessionFactory);
        }

        private TestDataMother mother;
    }
}