using System;
using System.Globalization;
using System.IO;

namespace GnuCashUtils.Web.Infrastucture
{
    public class FluentHtml
    {
        public FluentHtml()
        {
            document = new GenericHtmlElement ("html", null);
            this.head = new GenericHtmlElement("head", document);
            this.title = new GenericHtmlElement("title", head);
            head.AddChild(title);
            document.AddChild(head);
            this.body = new GenericHtmlElement("body", document);
            document.AddChild(body);
            currentElement = body;
        }

        public FluentHtml AddCssLink(string cssUrl)
        {
            GenericHtmlElement linkElement = new GenericHtmlElement("link", head);
            linkElement.SetAttribute ("rel", "stylesheet");
            linkElement.SetAttribute ("href", cssUrl);
            linkElement.SetAttribute ("type", "text/css");
            head.AddChild(linkElement);
            return this;
        }

        public FluentHtml AddScript (string scriptUrl)
        {
            GenericHtmlElement linkElement = new GenericHtmlElement ("script", head);
            linkElement.SetAttribute ("type", "text/javascript");
            linkElement.SetAttribute ("src", scriptUrl);
            linkElement.Text = "   ";
            head.AddChild (linkElement);
            return this;
        }

        public FluentHtml Close ()
        {
            currentElement = currentElement.Parent;
            return this;
        }

        public FluentHtml Div(string className)
        {
            AddGenericHtmlElement("div", className);
            return this;
        }

        public FluentHtml Link(string url, string text)
        {
            GenericHtmlElement linkElement = new GenericHtmlElement("a", currentElement);
            linkElement.SetAttribute("href", url);
            linkElement.Text = text;
            currentElement.AddChild(linkElement);

            return this;
        }

        public FluentHtml SetPageTitle (string pageTitle)
        {
            title.Text = pageTitle;
            return this;
        }

        public FluentHtml Table(string className)
        {
            AddGenericHtmlElement("table", className);
            return this;
        }

        public FluentHtml TableData()
        {
            AddGenericHtmlElement("td");
            return this;
        }

        public FluentHtml TableData(object value)
        {
            TableData();
            Text(value);
            return Close();
        }

        public FluentHtml TableHeader(string text)
        {
            AddGenericHtmlElement("th");
            currentElement.Text = text;
            return Close();
        }

        public FluentHtml TableRow()
        {
            AddGenericHtmlElement("tr");
            return this;
        }

        public FluentHtml Text(object value)
        {
            if (value != null)
                currentElement.Text = value.ToString();
            return this;
        }

        public FluentHtml Text (string format, params object[] args)
        {
            currentElement.Text = string.Format(
                format,
                args);
            return this;
        }

        public override string ToString ()
        {
            using (StringWriter writer = new StringWriter ())
            {
                HtmlWritingContext context = new HtmlWritingContext ();

                document.Write (writer, context);
                return writer.ToString();
            }
        }

        private void AddGenericHtmlElement(string elementName)
        {
            AddGenericHtmlElement(elementName, null);
        }

        private void AddGenericHtmlElement(string elementName, string className)
        {
            IHtmlElement divElement = new GenericHtmlElement(elementName, currentElement);
            currentElement.AddChild(divElement);

            if(false == String.IsNullOrEmpty(className))
                divElement.SetAttribute("class", className);

            currentElement = divElement;
        }

        private GenericHtmlElement body;
        private IHtmlElement currentElement;
        private IHtmlElement document;
        private GenericHtmlElement head;
        private GenericHtmlElement title;
    }
}