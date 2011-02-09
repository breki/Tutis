using System;
using System.IO;

namespace GnuCashUtils.Web.Infrastucture
{
    public interface IHtmlElement
    {
        IHtmlElement Parent { get; }
        string Text { get; set; }

        void AddChild(IHtmlElement childElement);
        void SetAttribute(string name, string value);
        void Write(StringWriter writer, HtmlWritingContext context);
    }
}