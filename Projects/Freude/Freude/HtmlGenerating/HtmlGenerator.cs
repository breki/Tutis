using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Brejc.Common;
using Freude.DocModel;

namespace Freude.HtmlGenerating
{
    public class HtmlGenerator
    {
        public string GenerateHtml(DocumentDef doc)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            using (StringWriter strWriter = new StringWriter(CultureInfo.InvariantCulture))
            using (XmlWriter writer = XmlWriter.Create(strWriter, settings))
            {
                foreach (IDocumentElement child in doc.Children)
                    RenderElement(child, writer);

                writer.Flush();
                return strWriter.ToString();
            }
        }

        private static void RenderElement(IDocumentElement element, XmlWriter writer)
        {
            if (element is HeaderElement)
                RenderHeaderElement ((HeaderElement)element, writer);
            else if (element is ImageElement)
                RenderImageElement ((ImageElement)element, writer);
            else if (element is ParagraphElement)
                RenderParagraphElement((ParagraphElement)element, writer);
            else if (element is TextElement)
                RenderTextElement ((TextElement)element, writer);
            else
                throw new NotImplementedException("Rendering of {0} element not yet implemented".Fmt(element.GetType().Name));
        }

        private static void RenderHeaderElement(HeaderElement element, XmlWriter writer)
        {
            writer.WriteStartElement ("h" + element.HeaderLevel.ToString(CultureInfo.InvariantCulture));
            writer.WriteValue(element.HeaderText);
            writer.WriteEndElement ();
        }

        private static void RenderImageElement(ImageElement element, XmlWriter writer)
        {
            writer.WriteStartElement("img");
            writer.WriteAttributeString("src", element.ImageUrl);
            writer.WriteEndElement();
        }

        private static void RenderParagraphElement (ParagraphElement element, XmlWriter writer)
        {
            writer.WriteStartElement ("p");
            foreach (IDocumentElement child in element.Children)
                RenderElement(child, writer);
            writer.WriteEndElement();
        }

        private static void RenderTextElement(TextElement element, XmlWriter writer)
        {
            writer.WriteValue(element.Text);
        }
    }
}