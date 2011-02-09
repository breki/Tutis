using System;
using System.Collections.Generic;

namespace GnuCashUtils.Web.Models
{
    public interface IBalancesCalculator
    {
        IDictionary<Guid, AccountWithBalanceHistory> CalculateAccountsBalancesForTheseDates(IList<DateTime> dates);
    }
}