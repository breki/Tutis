namespace Freude.Tests.FreudeTextParsingTests
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

        private readonly string text;
    }
}