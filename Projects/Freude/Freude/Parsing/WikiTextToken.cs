using System.Diagnostics.Contracts;
using System.Globalization;

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

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} ('{1}')", tokenType, text);
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
            Heading1Start,
            Heading1End,
            Heading2Start,
            Heading2End,
            Heading3Start,
            Heading3End,
            Heading4Start,
            Heading4End,
            Heading5Start,
            Heading5End,
            Heading6Start,
            Heading6End,
            HeadingAnchor,
            BulletList,
            NumberedList,
            Indent,
            ExternalLinkUrlLeadingSpace,
            ExternalLinkUrl
        }
    }
}