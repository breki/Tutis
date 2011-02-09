using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using GnuCashUtils.Framework;
using GnuCashUtils.Web.Models;

namespace GnuCashUtils.Web.Controllers
{
    public class AccountsController : Controller
    {
        public AccountsController(
            IAccountsRepository accountsRepository,
            IBalancesCalculator balancesCalculator)
        {
            this.accountsRepository = accountsRepository;
            this.balancesCalculator = balancesCalculator;
        }

        public ActionResult Details (Guid id)
        {
            Account account = accountsRepository.GetAccounts().ById(id);
            return View("accountdetails", account);
        }

        public ActionResult List()
        {
            List<DateTime> dates = new List<DateTime> ();
            dates.Add (DateTime.Now.Date);
            dates.Add (DateTime.Now.AddDays (-7).Date);
            dates.Add (DateTime.Now.AddMonths (-1).Date);
            dates.Add (DateTime.Now.AddMonths (-3).Date);
            dates.Add (DateTime.Now.AddYears (-1).Date);
            dates.Sort ();

            IDictionary<Guid, AccountWithBalanceHistory> accountsWithBalances 
                = balancesCalculator.CalculateAccountsBalancesForTheseDates (dates);

            AccountsList accountsList = new AccountsList(
                accountsRepository.RootAccount,
                accountsWithBalances);

            return View("accounts", accountsList);
        }

        private readonly IAccountsRepository accountsRepository;
        private readonly IBalancesCalculator balancesCalculator;
    }
}