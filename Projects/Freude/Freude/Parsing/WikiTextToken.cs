using System.Diagnostics.Contracts;

namespace Freude.Parsing
{
    public class WikiTextToken
    {
        public WikiTextToken(TokenType tokenType, string text, WikiTextTokenScopes scopes)
        {
            Contract.Requires(!string.IsNullOrEmpty(text));

            this.tokenType = tokenType;
            this.text = text;
            this.scopes = scopes;
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

        public WikiTextTokenScopes Scopes
        {
            get { return scopes; }
        }

        private readonly TokenType tokenType;
        private readonly string text;
        private readonly WikiTextTokenScopes scopes;

        public enum TokenType
        {
            Text,
            SingleSquareBracketsOpen,
            DoubleSquareBracketsOpen,
            SingleSquareBracketsClose,
            DoubleSquareBracketsClose,
            NamespaceSeparator,
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
            NumberedList
        }
    }
}