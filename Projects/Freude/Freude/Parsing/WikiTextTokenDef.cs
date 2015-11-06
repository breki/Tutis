using System;
using System.Diagnostics.Contracts;

namespace Freude.Parsing
{
    internal class WikiTextTokenDef
    {
        public WikiTextTokenDef (
            string tokenString, 
            WikiTextToken.TokenType tokenType, 
            WikiTextTokenScopes availableInScopes,
            Func<WikiTextTokenScopes, WikiTextTokenScopes> scopeModifierFunc = null)
        {
            Contract.Requires (!String.IsNullOrEmpty (tokenString));

            this.tokenString = tokenString;
            this.tokenType = tokenType;
            this.availableInScopes = availableInScopes;
            this.scopeModifierFunc = scopeModifierFunc;
        }

        public string TokenString
        {
            get { return tokenString; }
        }

        public WikiTextToken.TokenType TokenType
        {
            get { return tokenType; }
        }

        public WikiTextTokenScopes AvailableInScopes
        {
            get { return availableInScopes; }
        }

        public int ModifyScope(int currentScope)
        {
            if (scopeModifierFunc != null)
                return (int)scopeModifierFunc((WikiTextTokenScopes)currentScope);

            return currentScope;
        }

        private readonly string tokenString;
        private readonly WikiTextToken.TokenType tokenType;
        private readonly WikiTextTokenScopes availableInScopes;
        private readonly Func<WikiTextTokenScopes, WikiTextTokenScopes> scopeModifierFunc;
    }
}