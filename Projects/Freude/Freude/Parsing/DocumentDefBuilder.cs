using System;
using System.Diagnostics.Contracts;
using Freude.DocModel;

namespace Freude.Parsing
{
    public class DocumentDefBuilder
    {
        public DocumentDef Document
        {
            get { return doc; }
        }

        public ParagraphElement CurrentParagraph
        {
            get { return currentParagraph; }
        }

        public TextElement.TextStyle? CurrentParagraphTextStyle
        {
            get { return currentParagraphTextStyle; }
            set { currentParagraphTextStyle = value; }
        }

        public void AddRootChild (IDocumentElement element)
        {
            Contract.Requires(element != null);
            doc.AddChild (element);
        }

        public void AddToParagraph (IDocumentElement elementToAdd)
        {
            Contract.Requires (elementToAdd != null);
            Contract.Ensures (currentParagraph != null);

            CreateParagraphIfNoneIsAlreadyOpen();
            currentParagraph.AddChild (elementToAdd);
        }

        public void AddTextToParagraph (string text, bool isFirstElementOfContinuedLine)
        {
            Contract.Requires(text != null);

            CreateParagraphIfNoneIsAlreadyOpen();

            int childrenCount = currentParagraph.ChildrenCount;
            if (childrenCount > 0)
            {
                IDocumentElement lastChild = currentParagraph.Children[childrenCount - 1];
                TextElement textChild = lastChild as TextElement;
                if (textChild != null)
                {
                    // ReSharper disable once PossibleInvalidOperationException
                    if (textChild.Style == currentParagraphTextStyle.Value)
                        textChild.AppendText (text);
                    else
                        AddTextElementToParagraph (
                            currentParagraph, text, currentParagraphTextStyle.Value, isFirstElementOfContinuedLine);
                }
                else if (lastChild is InternalLinkElement || lastChild is ExternalLinkElement)
                    // ReSharper disable once PossibleInvalidOperationException
                    AddTextElementToParagraph (
                        currentParagraph, text, currentParagraphTextStyle.Value, isFirstElementOfContinuedLine);
                else
                    throw new InvalidOperationException ("BUG: is this possible?");
            }
            else
            {
                // ReSharper disable once PossibleInvalidOperationException
                AddToParagraph (CreateTextElement (text, currentParagraphTextStyle.Value));
            }
        }

        public void StartNewParagraph(ParagraphElement.ParagraphType paragraphType, int indentation)
        {
            Contract.Requires(indentation >= 0);

            FinalizeCurrentParagraph();

            currentParagraph = new ParagraphElement (paragraphType, indentation);
            currentParagraphTextStyle = TextElement.TextStyle.Regular;
            doc.AddChild (currentParagraph);
        }

        public void FinalizeCurrentParagraph ()
        {
            if (currentParagraph != null)
            {
                currentParagraph.Trim();
                currentParagraph = null;
            }
        }

        private static void AddTextElementToParagraph (
            ParagraphElement paragraph, 
            string text, 
            TextElement.TextStyle currentStyle,
            bool insertSpaceBefore)
        {
            Contract.Requires (paragraph != null);
            Contract.Requires (text != null);
            TextElement newStyleChild = CreateTextElement (
                insertSpaceBefore ? ' ' + text : text, 
                currentStyle);
            paragraph.AddChild (newStyleChild);
        }

        private static TextElement CreateTextElement (string text, TextElement.TextStyle currentStyle)
        {
            Contract.Requires (text != null);
            return new TextElement (text, currentStyle);
        }

        private void CreateParagraphIfNoneIsAlreadyOpen()
        {
            if (currentParagraph != null) 
                return;

            currentParagraph = new ParagraphElement (ParagraphElement.ParagraphType.Regular, 0);
            currentParagraphTextStyle = currentParagraphTextStyle ?? TextElement.TextStyle.Regular;
            doc.AddChild (currentParagraph);
        }

        private readonly DocumentDef doc = new DocumentDef ();
        private ParagraphElement currentParagraph;
        private TextElement.TextStyle? currentParagraphTextStyle;
    }
}