using System;
using System.Collections.Generic;
using System.IO;

namespace GnuCashUtils.Web.Infrastucture
{
    public class GenericHtmlElement : IHtmlElement
    {
        public GenericHtmlElement(string elementName, IHtmlElement parent)
        {
            this.elementName = elementName;
            this.parent = parent;
        }

        public string ElementName
        {
            get { return elementName; }
        }

        public IHtmlElement Parent
        {
            get { return parent; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public void AddChild(IHtmlElement childElement)
        {
            children.Add(childElement);
        }

        public void SetAttribute(string name, string value)
        {
            attributes[name] = value;
        }

        public void Write (StringWriter writer, HtmlWritingContext context)
        {
            Indent(writer, context);
            writer.Write("<{0}", elementName);

            foreach (KeyValuePair<string, string> attribute in attributes)
                writer.Write(" {0}='{1}'", attribute.Key, attribute.Value);

            if (children.Count > 0 || text != null)
            {
                writer.WriteLine(">");

                context.Depth++;

                if (false == String.IsNullOrEmpty (text))
                {
                    Indent(writer, context);
                    writer.WriteLine(text);
                }

                foreach (IHtmlElement child in children)
                    child.Write (writer, context);
                
                context.Depth--;

                Indent (writer, context);
                writer.WriteLine ("</{0}>", elementName);
            }
            else
            {
                writer.WriteLine("/>");
            }
        }

        private static void Indent (StringWriter writer, HtmlWritingContext context)
        {
            writer.Write(new string('\t', context.Depth));
        }

        private Dictionary<string, string> attributes = new Dictionary<string, string> ();
        private List<IHtmlElement> children = new List<IHtmlElement>();
        private readonly string elementName;
        private IHtmlElement parent;
        private string text;
    }
}