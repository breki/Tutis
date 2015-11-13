using System;
using System.Collections.Generic;
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
            paragraphsStack.Clear();
            OnDocumentBegin(doc);

            foreach (IDocumentElement el in doc.Children)
            {
                ParagraphElement paragraphEl = el as ParagraphElement;
                HeadingElement headingEl = el as HeadingElement;

                if (paragraphEl != null)
                    ProcessParagraphElement(paragraphEl);
                else if (headingEl != null)
                    ProcessHeadingElement(headingEl);
                else
                    throw new NotImplementedException("todo next: {0}".Fmt(el.GetType().Name));
            }

            EnsureParagraphStackIsCleared ();

            OnDocumentEnd (doc);
        }

        protected virtual void OnDocumentBegin(DocumentDef doc)
        {
            Contract.Requires(doc != null);
            Contract.Requires(!ProcessingFinished);
            Contract.Ensures (!ProcessingFinished);
        }

        protected virtual void OnHeadingElement(HeadingElement headingEl)
        {
            Contract.Requires(headingEl != null);
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

        protected virtual void OnNumberedListBegin(ParagraphElement paragraphEl)
        {
            Contract.Requires (paragraphEl != null);
            Contract.Requires (!ProcessingFinished);
            Contract.Ensures (!ProcessingFinished);
        }

        protected virtual void OnNumberedListItemBegin(ParagraphElement paragraphEl)
        {
            Contract.Requires (paragraphEl != null);
            Contract.Requires (!ProcessingFinished);
            Contract.Ensures (!ProcessingFinished);
        }

        protected virtual void OnNumberedListItemEnd(ParagraphElement paragraphEl)
        {
            Contract.Requires (paragraphEl != null);
            Contract.Requires (!ProcessingFinished);
            Contract.Ensures (!ProcessingFinished);
        }

        protected virtual void OnNumberedListEnd(ParagraphElement paragraphEl)
        {
            Contract.Requires (paragraphEl != null);
            Contract.Requires (!ProcessingFinished);
            Contract.Ensures (!ProcessingFinished);
        }

        protected virtual void OnTextElement(TextElement textEl)
        {
            Contract.Requires(textEl != null);
            Contract.Requires (!ProcessingFinished);
            Contract.Ensures (!ProcessingFinished);
        }

        protected virtual void OnInternalLinkElement (InternalLinkElement linkEl)
        {
            Contract.Requires(linkEl != null);
            Contract.Requires (!ProcessingFinished);
            Contract.Ensures (!ProcessingFinished);
        }

        protected virtual void OnExternalLinkElement(ExternalLinkElement linkEl)
        {
            Contract.Requires (linkEl != null);
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
            
            switch (paragraphEl.Type)
            {
                case ParagraphElement.ParagraphType.Regular:
                    ProcessRegularParagraphElement(paragraphEl);
                    break;
                case ParagraphElement.ParagraphType.Bulleted:
                    ProcessBulletedParagraphElement(paragraphEl);
                    break;
                case ParagraphElement.ParagraphType.Numbered:
                    ProcessNumberedParagraphElement(paragraphEl);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private void ProcessRegularParagraphElement(ParagraphElement paragraphEl)
        {
            EnsureParagraphStackIsCleared ();

            OnParagraphBegin (paragraphEl);
            ProcessParagraphContents(paragraphEl);
            OnParagraphEnd (paragraphEl);
        }

        private void ProcessBulletedParagraphElement(ParagraphElement paragraphEl)
        {
            throw new NotImplementedException();
        }

        private void ProcessNumberedParagraphElement(ParagraphElement paragraphEl)
        {
            bool beginsNewList = EnsureParagraphStackIsClearedUntil(paragraphEl);

            if (beginsNewList)
                OnNumberedListBegin(paragraphEl);

            OnNumberedListItemBegin(paragraphEl);
            ProcessParagraphContents (paragraphEl);
            OnNumberedListItemEnd (paragraphEl);
        }

        private void ProcessParagraphContents(ParagraphElement paragraphEl)
        {
            foreach (IDocumentElement el in paragraphEl.Children)
            {
                TextElement textEl = el as TextElement;
                InternalLinkElement internalLinkEl = el as InternalLinkElement;
                ExternalLinkElement externalLinkEl = el as ExternalLinkElement;

                if (textEl != null)
                    ProcessTextElement(textEl);
                else if (internalLinkEl != null)
                    ProcessInternalLinkElement(internalLinkEl);
                else if (externalLinkEl != null)
                    ProcessExternalLinkElement(externalLinkEl);
                else
                    throw new NotImplementedException("todo next:");
            }
        }

        private void EnsureParagraphStackIsCleared()
        {
            while (paragraphsStack.Count > 0)
            {
                ParagraphElement stackedParagraph = paragraphsStack.Pop ();

                switch (stackedParagraph.Type)
                {
                    case ParagraphElement.ParagraphType.Bulleted:
                        throw new NotImplementedException("todo next:");
                    case ParagraphElement.ParagraphType.Numbered:
                        OnNumberedListEnd(stackedParagraph);
                        break;
                    default:
                        throw new InvalidOperationException("BUG");
                }
            }
        }

        private bool EnsureParagraphStackIsClearedUntil(ParagraphElement paragraphEl)
        {
            Contract.Requires(paragraphEl != null);

            while (paragraphsStack.Count > 0)
            {
                ParagraphElement stackedParagraph = paragraphsStack.Pop();
                if (stackedParagraph.Type == paragraphEl.Type && stackedParagraph.Indentation == paragraphEl.Indentation)
                {
                    paragraphsStack.Push(paragraphEl);
                    return false;
                }

                throw new NotImplementedException("todo next:");
            }

            paragraphsStack.Push (paragraphEl);
            return true;
        }

        private void ProcessHeadingElement(HeadingElement headingEl)
        {
            Contract.Requires(headingEl != null);

            EnsureParagraphStackIsCleared ();
            OnHeadingElement(headingEl);
        }

        private void ProcessTextElement(TextElement textEl)
        {
            Contract.Requires(textEl != null);

            OnTextElement(textEl);
        }

        private void ProcessInternalLinkElement(InternalLinkElement linkEl)
        {
            Contract.Requires(linkEl != null);

            OnInternalLinkElement(linkEl);
        }

        private void ProcessExternalLinkElement(ExternalLinkElement linkEl)
        {
            Contract.Requires(linkEl != null);

            OnExternalLinkElement(linkEl);
        }

        private readonly Stack<ParagraphElement> paragraphsStack = new Stack<ParagraphElement> ();
    }
}