using System.Web.Mvc;
using GnuCashUtils.Framework;
using GnuCashUtils.Web.Controllers;
using GnuCashUtils.Web.Models;
using MbUnit.Framework;

namespace GnuCashUtils.Test.WebTests.ControllerTests
{
    [TestFixture]
    public class AccountsTests
    {
        [Test]
        public void ListAccounts()
        {
            ActionResult actionResult = controller.List();
        }

        /// <summary>Test case setup code.</summary>
        [SetUp]
        public void Setup()
        {
            Book book = new XmlBookReader ("../../../Data/Igor.xml").Read ();
            IAccountsRepository accountsRepository = new AccountsRepository(book);
            IBalancesCalculator balancesCalculator = new BalancesCalculator(book);

            controller = new AccountsController (accountsRepository, balancesCalculator);            
        }

        private AccountsController controller;
    }
}