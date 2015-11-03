using System.Diagnostics.Contracts;

namespace Freude.Parsing
{
    public class WikiTextToken
    {
        public WikiTextToken(TokenType tokenType, string text)
        {
            Contract.Requires(!string.IsNullOrEmpty(text));

            this.tokenType = tokenType;
            this.text = text;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public TokenType Type
        {
            get { return tokenType; }
        }

        public string Text
        {
            get { return text; }
        }

        private readonly TokenType tokenType;
        private readonly string text;

        public enum TokenType
        {
            Text,
            SingleSquareBracketsOpen,
            DoubleSquareBracketsOpen,
            SingleSquareBracketsClose,
            DoubleSquareBracketsClose,
            Pipe,
            DoubleApostrophe,
            TripleApostrophe,
            Header1Start,
            Header1End,
            Header2Start,
            Header2End,
            Header3Start,
            Header3End,
            Header4Start,
            Header4End,
            Header5Start,
            Header5End,
            Header6Start,
            Header6End,
            HeaderAnchor,
            BulletList,
            NumberedList,
        }
    }
}