namespace Parser.Coco
{
    public class Literal : ILabelPart
    {
        public Literal(string text)
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