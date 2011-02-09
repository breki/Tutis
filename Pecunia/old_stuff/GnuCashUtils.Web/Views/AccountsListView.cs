using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using GnuCashUtils.Framework;
using GnuCashUtils.Web.Infrastucture;
using GnuCashUtils.Web.Models;

namespace GnuCashUtils.Web.Views
{
    public class AccountsListView : VidiViewBase<AccountsList>
    {
        protected override void RenderHtml (ViewDataDictionary<AccountsList> viewData, FluentHtml fluentHtml)
        {
            fluentHtml
                .SetPageTitle("Accounts")
                .AddCssLink("gnu/Content/gc.css")
                .AddScript("gnu/Scripts/jquery-1.3.2.js")
                .Div("accounts")
                .Table("accountsTable")
                .TableRow()
                .TableHeader("Account")
                .TableHeader("Description")
                .TableHeader("Total")
                .TableHeader("Balance 1w")
                .TableHeader("Balance 1m")
                .TableHeader("Balance 3m")
                .TableHeader("Balance 1y")
                .Close();

            RenderAccount (viewData.Model.RootAccount, viewData.Model, fluentHtml);

            fluentHtml
                .Close()
                .Close();
        }

        private static void RenderAccount (Account account, AccountsList accountsList, FluentHtml fluentHtml)
        {
            Guid accountId = account.Id;
            AccountWithBalanceHistory accountWithHistory = null;

            if (accountsList.AccountsWithBalances.ContainsKey(accountId))
                accountWithHistory = accountsList.AccountsWithBalances[accountId];

            StringBuilder accountLinkIndent = new StringBuilder();
            for (int i = 0; i < account.Depth * 3; i++)
                accountLinkIndent.Append("&nbsp;");

            fluentHtml
                .TableRow()
                .TableData()
                .Text(accountLinkIndent)
                .Link("/accounts/details/" + account.Id, account.Name)
                .Close()
                .TableData(account.Description);

            if (accountWithHistory != null)
            {
                for (int i = accountWithHistory.Balances.Count - 1; i >= 0; i--)
                {
                    if (accountWithHistory.Balances[i].HasValue)
                        fluentHtml
                            .TableData (accountWithHistory.Balances[i].Value);
                }
            }

            fluentHtml.Close();

            foreach (Account childAccount in account.ChildAccounts)
                RenderAccount(childAccount, accountsList, fluentHtml);
        }
    }
}