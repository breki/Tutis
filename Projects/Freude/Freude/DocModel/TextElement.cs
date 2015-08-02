using System;

namespace Freude.DocModel
{
    public class TextElement : IDocumentElement
    {
        public TextElement(string text)
        {
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

        private string text;
    }
}