using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GnuCashUtils.Framework;
using GnuCashUtils.Web.Models;
using MbUnit.Framework;

namespace GnuCashUtils.Test.WebTests.RepositoryTests
{
    [TestFixture]
    public class TransactionFiltersTests
    {
        [Test]
        public void OnOrAfterDateFilterReturnsAppropriateRecords()
        {
            DateTime date = new DateTime(2008, 06, 06);

            IQueryable<Transaction> transactions = repository.GetTransactions().OnOrAfter(date);
            Assert.IsTrue(transactions.Count() > 0);
            Assert.IsFalse(transactions.Any(t => t.DatePosted.Date < date.Date));
        }

        [Test]
        public void BeforeDateFilterReturnsAppropriateRecords()
        {
            DateTime date = new DateTime(2008, 06, 06);

            IQueryable<Transaction> transactions = repository.GetTransactions().Before(date);
            Assert.IsTrue(transactions.Count() > 0);
            Assert.IsFalse(transactions.Any(t => t.DatePosted.Date >= date.Date));
        }

        /// <summary>Test case setup code.</summary>
        [SetUp]
        public void Setup()
        {
            Book book = new XmlBookReader("../../../Data/Igor.xml").Read();
            repository = new TransactionsRepository(book);
        }

        private ITransactionsRepository repository;
    }
}
