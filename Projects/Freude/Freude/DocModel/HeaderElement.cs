namespace Freude.DocModel
{
    public class HeaderElement : IDocumentElement
    {
        public HeaderElement(string headerText, int headerLevel)
        {
            this.headerText = headerText;
            this.headerLevel = headerLevel;
        }

        public string HeaderText
        {
            get { return headerText; }
        }

        public int HeaderLevel
        {
            get { return headerLevel; }
        }

        public string AnchorId
        {
            get; set; 
        }

        private readonly string headerText;
        private readonly int headerLevel;
    }
}