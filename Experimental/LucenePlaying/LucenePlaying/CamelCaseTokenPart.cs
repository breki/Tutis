namespace LucenePlaying
{
    public class CamelCaseTokenPart
    {
        public CamelCaseTokenPart(string tokenText, int offset)
        {
            this.text = tokenText;
            this.offset = offset;
        }

        public string Text
        {
            get { return text; }
        }

        public int Offset
        {
            get { return offset; }
        }

        public CamelCaseTokenPart CreatePart (int startIndex, int endIndexPlusOne)
        {
            int partLength = endIndexPlusOne - startIndex;

            if (partLength == 0 || partLength == 1)
                return null;

            string partText = text.Substring(startIndex, partLength);
            if (partText.StartsWith("I"))
            {
                partText = partText.Substring(1);
                startIndex++;
            }

            return new CamelCaseTokenPart(
                partText.ToLowerInvariant(),
                offset + startIndex);
        }

        private readonly string text;
        private readonly int offset;        
    }
}