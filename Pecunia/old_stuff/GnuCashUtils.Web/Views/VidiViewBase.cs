using System.IO;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using GnuCashUtils.Web.Infrastucture;

namespace GnuCashUtils.Web.Views
{
    public abstract class VidiViewBase<TModel> : ViewPage<TModel>, IView where TModel : class
    {
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            FluentHtml fluentHtml = new FluentHtml();

            RenderHtml(new ViewDataDictionary<TModel>(viewContext.ViewData), fluentHtml);
            //RenderHtml(ViewData, fluentHtml);

            writer.Write(fluentHtml);
        }

        protected abstract void RenderHtml(ViewDataDictionary<TModel> viewData, FluentHtml fluentHtml);
    }
}