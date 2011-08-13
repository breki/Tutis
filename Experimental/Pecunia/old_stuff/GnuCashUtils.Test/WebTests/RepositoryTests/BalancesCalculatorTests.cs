using System;
using System.Collections.Generic;
using GnuCashUtils.Framework;
using GnuCashUtils.Web.Models;
using MbUnit.Framework;

namespace GnuCashUtils.Test.WebTests.RepositoryTests
{
    public class BalancesCalculatorTests
    {
        [Test]
        public void Test()
        {
            Book book = new XmlBookReader ("../../../Data/Igor.xml").Read ();
            IBalancesCalculator calculator = new BalancesCalculator(book);
            IDictionary<Guid, AccountWithBalanceHistory> dictionary = calculator.CalculateAccountsBalancesForTheseDates(new List<DateTime>() {DateTime.Now});

            decimal? dec = dictionary[book.RootAccount.Id].Balances[0];
        }
    }
}