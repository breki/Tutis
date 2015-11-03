using System;
using System.Diagnostics.Contracts;

namespace Freude.DocModel
{
    public class TextElement : IDocumentElement
    {
        public TextElement(string text)
        {
            Contract.Requires(text != null);
            this.text = text;
        }

        public string Text
        {
            get { return text; }
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
    }
}