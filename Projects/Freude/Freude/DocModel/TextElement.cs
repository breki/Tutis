using System;
using System.Diagnostics.Contracts;

namespace Freude.DocModel
{
    public class TextElement : IDocumentElement
    {
        public TextElement(string text, TextStyle style = TextStyle.Regular)
        {
            Contract.Requires(text != null);
            this.text = text;
            this.style = style;
        }

        public string Text
        {
            get { return text; }
        }

        public TextStyle Style
        {
            get { return style; }
        }

        public void AppendText(string textToAppend)
        {
            if (textToAppend == null)
                throw new ArgumentNullException("textToAppend");

            text += ' ' + textToAppend.Trim();
        }

        public void TrimStart()
        {
            text = text.TrimStart();
        }

        public void TrimEnd()
        {
            text = text.TrimEnd();
        }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(text != null);
        }

        private string text;
        private readonly TextStyle style;

        public enum TextStyle
        {
            Regular,
            Bold,
            Italic,
            BoldItalic,
        }
    }
}