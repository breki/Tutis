using System;
using System.Web.Mvc;
using GnuCashUtils.Framework;
using GnuCashUtils.Web.Infrastucture;

namespace GnuCashUtils.Web.Views
{
    public class AccountDetailsView : VidiViewBase<Account>
    {
        protected override void RenderHtml(ViewDataDictionary<Account> viewData, FluentHtml fluentHtml)
        {
            fluentHtml
                .SetPageTitle("Accounts")
                .AddCssLink("gnu/Content/Site.css")
                .AddScript("gnu/Scripts/jquery-1.3.2.js")
                .Div("account");

            RenderAccount(viewData.Model, fluentHtml);
        }

        private void RenderAccount(Account account, FluentHtml fluentHtml)
        {
            fluentHtml.Text("Account details for {0}", account.Name);
        }
    }
}