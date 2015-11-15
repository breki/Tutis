using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI;
using Brejc.Common;
using Freude.DocModel;
using Freude.DocProcessing;

namespace Freude.Tests.DocumentProcessingTests
{
    public class TestDocumentProcessor : DocumentProcessorBase, IDisposable
    {
        public string ResultHtml
        {
            get { return resultHtml; }
        }

        public string CssParagraphIndentClass
        {
            get { return cssParagraphIndentClass; }
            set { cssParagraphIndentClass = value; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override void OnDocumentBegin(DocumentDef doc)
        {
            textBuilder = new StringBuilder();
            textWriter = new StringWriter(textBuilder, CultureInfo.InvariantCulture);
            writer = new HtmlTextWriter(textWriter, "  ");

            writer.RenderBeginTag(HtmlTextWriterTag.Body);
        }

        protected override void OnHeadingElement(HeadingElement headingEl)
        {
            if (headingEl.AnchorId != null)
                writer.AddAttribute("id", headingEl.AnchorId);

            writer.RenderBeginTag("h{0}".Fmt(headingEl.HeadingLevel));
            writer.WriteEncodedText(headingEl.HeadingText);
            writer.RenderEndTag();
        }

        protected override void OnParagraphBegin(ParagraphElement el)
        {
            if (el.Indentation > 0)
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "{0}-{1}".Fmt(CssParagraphIndentClass, el.Indentation));

            writer.RenderBeginTag(HtmlTextWriterTag.P);
        }

        protected override void OnParagraphEnd(ParagraphElement el)
        {
            writer.RenderEndTag();
        }

        protected override void OnNumberedListBegin(ParagraphElement paragraphEl)
        {
            writer.RenderBeginTag (HtmlTextWriterTag.Ol);
        }

        protected override void OnNumberedListItemBegin(ParagraphElement paragraphEl)
        {
            writer.RenderBeginTag (HtmlTextWriterTag.Li);
        }

        protected override void OnNumberedListItemEnd(ParagraphElement paragraphEl)
        {
            writer.RenderEndTag ();
        }

        protected override void OnNumberedListEnd(ParagraphElement paragraphEl)
        {
            writer.RenderEndTag ();
        }

        protected override void OnBulletListBegin(ParagraphElement paragraphEl)
        {
            writer.RenderBeginTag (HtmlTextWriterTag.Ul);
        }

        protected override void OnBulletListItemBegin(ParagraphElement paragraphEl)
        {
            writer.RenderBeginTag (HtmlTextWriterTag.Li);
        }

        protected override void OnBulletListItemEnd(ParagraphElement paragraphEl)
        {
            writer.RenderEndTag ();
        }

        protected override void OnBulletListEnd(ParagraphElement paragraphEl)
        {
            writer.RenderEndTag ();
        }

        protected override void OnTextElement(TextElement textEl)
        {
            base.OnTextElement(textEl);

            switch (textEl.Style)
            {
                case TextElement.TextStyle.Regular:
                    break;
                case TextElement.TextStyle.Bold:
                    writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    break;
                case TextElement.TextStyle.Italic:
                    writer.RenderBeginTag(HtmlTextWriterTag.Em);
                    break;
                case TextElement.TextStyle.BoldItalic:
                    writer.RenderBeginTag(HtmlTextWriterTag.Strong);
                    writer.RenderBeginTag(HtmlTextWriterTag.Em);
                    break;
                default:
                    throw new NotSupportedException();
            }

            writer.WriteEncodedText(textEl.Text);

            switch (textEl.Style)
            {
                case TextElement.TextStyle.Regular:
                    break;
                case TextElement.TextStyle.Bold:
                    writer.RenderEndTag();
                    break;
                case TextElement.TextStyle.Italic:
                    writer.RenderEndTag();
                    break;
                case TextElement.TextStyle.BoldItalic:
                    writer.RenderEndTag();
                    break;
                default:
                    throw new NotSupportedException ();
            }
        }

        protected override void OnInternalLinkElement(InternalLinkElement linkEl)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Href, "http://google.com/{0}".Fmt(linkEl.LinkId));
            writer.RenderBeginTag(HtmlTextWriterTag.A);

            if (linkEl.LinkDescription != null)
                writer.WriteEncodedText(linkEl.LinkDescription);
            else
                writer.WriteEncodedText(linkEl.LinkId.ToString());

            writer.RenderEndTag();
        }

        protected override void OnExternalLinkElement(ExternalLinkElement linkEl)
        {
            writer.AddAttribute (HtmlTextWriterAttribute.Href, linkEl.Url.ToString());
            writer.RenderBeginTag (HtmlTextWriterTag.A);

            if (linkEl.LinkDescription != null)
                writer.WriteEncodedText(linkEl.LinkDescription);
            else
                writer.Write(linkEl.Url);

            writer.RenderEndTag ();
        }

        protected override void OnDocumentEnd(DocumentDef doc)
        {
            writer.RenderEndTag();
            writer.Flush();
            resultHtml = textWriter.ToString();

            DisposeOfManagedResources();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                DisposeOfManagedResources();

            disposed = true;
        }

        private void DisposeOfManagedResources()
        {
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }

            if (textWriter != null)
            {
                textWriter.Dispose();
                textWriter = null;
            }
        }

        private bool disposed;
        private HtmlTextWriter writer;
        private TextWriter textWriter;
        private StringBuilder textBuilder;
        private string resultHtml;
        private string cssParagraphIndentClass = "indent";
    }
}