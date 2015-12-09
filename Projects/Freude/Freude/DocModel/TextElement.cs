using System;
using System.Diagnostics.Contracts;
using System.Text;

namespace Freude.DocModel
{
    public class TextElement : IDocumentElement
    {
        public TextElement(string text, TextStyle style = TextStyle.Regular)
        {
            Contract.Requires(text != null);
            textBuilder = new StringBuilder(text);
            this.style = style;
        }

        public string Text
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);

                return textBuilder.ToString();
            }
        }

        public TextStyle Style
        {
            get { return style; }
        }

        public void AppendText(string textToAppend)
        {
            if (textToAppend == null)
                throw new ArgumentNullException("textToAppend");

            bool existingTextEndsWithWhitespace = textBuilder.Length > 0 && textBuilder[textBuilder.Length-1] == ' ';
            bool textToAppendEndsWithWhitespace = textToAppend.EndsWith(" ", StringComparison.Ordinal);

            if (!existingTextEndsWithWhitespace)
                textBuilder.Append(' ');

            textBuilder.Append(textToAppend.Trim());
            if (textToAppendEndsWithWhitespace)
                textBuilder.Append(' ');
        }

        public void TrimStart()
        {
            textBuilder = new StringBuilder(textBuilder.ToString().TrimStart());
        }

        public void TrimEnd()
        {
            textBuilder = new StringBuilder (textBuilder.ToString ().TrimEnd());
        }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(textBuilder != null);
        }

        private readonly TextStyle style;
        private StringBuilder textBuilder;

        public enum TextStyle
        {
            Regular,
            Bold,
            Italic,
            BoldItalic,
        }
    }
}