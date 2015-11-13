namespace Freude.DocModel
{
    public class HeadingElement : IDocumentElement
    {
        public HeadingElement(string headingText, int headingLevel)
        {
            this.headingText = headingText;
            this.headingLevel = headingLevel;
        }

        public string HeadingText
        {
            get { return headingText; }
        }

        public int HeadingLevel
        {
            get { return headingLevel; }
        }

        public string AnchorId
        {
            get; set; 
        }

        private readonly string headingText;
        private readonly int headingLevel;
    }
}