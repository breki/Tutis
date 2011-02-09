using System;
using System.Web.Mvc;
using GnuCashUtils.Web.Views;

namespace GnuCashUtils.Web.Infrastucture
{
    public class VidiViewEngine : IViewEngine
    {
        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            throw new NotImplementedException();
        }

        public ViewEngineResult FindView(
            ControllerContext controllerContext, 
            string viewName, 
            string masterName, 
            bool useCache)
        {
            if (viewName.Equals("Accounts", StringComparison.OrdinalIgnoreCase))
                return new ViewEngineResult(new AccountsListView(), this);
            else if (viewName.Equals("AccountDetails", StringComparison.OrdinalIgnoreCase))
                return new ViewEngineResult(new AccountDetailsView(), this);

            return null;
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
        }
    }
}