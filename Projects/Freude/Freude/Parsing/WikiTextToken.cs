using System.Diagnostics.Contracts;

namespace Freude.Parsing
{
    public class WikiTextToken
    {
        public WikiTextToken(TokenType tokenType, string text = null)
        {
            Contract.Requires(tokenType != TokenType.Text || (tokenType == TokenType.Text && !string.IsNullOrEmpty(text)));

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

        //[ContractInvariantMethod]
        //private void Invariant()
        //{
            
        //}

        private readonly TokenType tokenType;
        private readonly string text;

        public enum TokenType
        {
            Text,
            DoubleSquareBracketsOpen,
            DoubleSquareBracketsClose,
        }
    }
}