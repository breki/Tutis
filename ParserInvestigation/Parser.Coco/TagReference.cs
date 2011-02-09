namespace Parser.Coco
{
    public class TagReference : ILabelPart
    {
        public TagReference(string tagName)
        {
            this.tagName = tagName;
        }

        public string TagName
        {
            get { return tagName; }
        }

        private readonly string tagName;
    }
}