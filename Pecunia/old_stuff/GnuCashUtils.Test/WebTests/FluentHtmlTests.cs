using System;
using System.Windows.Forms;
using GnuCashUtils.Web.Infrastucture;
using MbUnit.Framework;

namespace GnuCashUtils.Test.WebTests
{
    [TestFixture]
    public class FluentHtmlTests
    {
        [Test]
        public void Test()
        {
            FluentHtml fluentHtml = new FluentHtml();
            fluentHtml
                .Div("accounts");
            fluentHtml
                .Div("account")
                .Text("test")
                .Close();

            string html = fluentHtml.ToString();
        }        
    }
}