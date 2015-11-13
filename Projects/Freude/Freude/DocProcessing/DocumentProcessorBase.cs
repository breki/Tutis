using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Freude.DocModel;

namespace Freude.DocProcessing
{
    public class DocumentProcessorBase
    {
        public bool ProcessingFinished { get; private set; }

        public void ProcessDocument(DocumentDef doc)
        {
            Contract.Requires(doc != null);
            Contract.Ensures (ProcessingFinished);

            ProcessingFinished = false;
            OnDocumentBegin(doc);

            foreach (IDocumentElement el in doc.Children)
            {
                if (el is ParagraphElement)
                    ProcessParagraphElement((ParagraphElement)el);
                else if (el is HeaderElement)
                    ProcessHeaderElement((HeaderElement)el);
                else
                    throw new NotImplementedException("todo next: {0}".Fmt(el.GetType().Name));
            }

            OnDocumentEnd (doc);
        }

        protected virtual void OnDocumentBegin(DocumentDef doc)
        {
            Contract.Requires(doc != null);
            Contract.Requires(!ProcessingFinished);
            Contract.Ensures (!ProcessingFinished);
        }

        protected virtual void OnHeaderElement(HeaderElement headerEl)
        {
            Contract.Requires(headerEl != null);
            Contract.Requires (!ProcessingFinished);
            Contract.Ensures (!ProcessingFinished);
        }

        protected virtual void OnParagraphBegin(ParagraphElement el)
        {
            Contract.Requires(el != null);
            Contract.Requires (!ProcessingFinished);
            Contract.Ensures (!ProcessingFinished);
        }

        protected virtual void OnParagraphEnd(ParagraphElement el)
        {
            Contract.Requires(el != null);
            Contract.Requires (!ProcessingFinished);
            Contract.Ensures (!ProcessingFinished);
        }

        protected virtual void OnTextElement(TextElement textEl)
        {
            Contract.Requires(textEl != null);
            Contract.Requires (!ProcessingFinished);
            Contract.Ensures (!ProcessingFinished);
        }

        protected virtual void OnDocumentEnd (DocumentDef doc)
        {
            Contract.Requires(doc != null);
            Contract.Requires (!ProcessingFinished);
            Contract.Ensures (ProcessingFinished);

            ProcessingFinished = true;
        }

        private void ProcessParagraphElement(ParagraphElement paragraphEl)
        {
            Contract.Requires(paragraphEl != null);
            OnParagraphBegin(paragraphEl);

            foreach (IDocumentElement el in paragraphEl.Children)
            {
                if (el is ParagraphElement)
                    ProcessParagraphElement ((ParagraphElement)el);
                else if (el is TextElement)
                    ProcessTextElement((TextElement)el);
                else
                    throw new NotImplementedException ("todo next:");
            }

            OnParagraphEnd(paragraphEl);
        }

        private void ProcessHeaderElement(HeaderElement headerEl)
        {
            Contract.Requires(headerEl != null);

            OnHeaderElement(headerEl);
        }

        private void ProcessTextElement(TextElement textEl)
        {
            Contract.Requires(textEl != null);

            OnTextElement(textEl);
        }
    }
}