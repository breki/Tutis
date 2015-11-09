using System;
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace Freude.Parsing
{
    internal class WikiTextTokenDef
    {
        public WikiTextTokenDef (
            string tokenString, 
            bool isRegexToken,
            WikiTextToken.TokenType tokenType, 
            WikiTextTokenScopes availableInScopes,
            Func<WikiTextTokenScopes, WikiTextTokenScopes> scopeModifierFunc = null)
        {
            Contract.Requires (!String.IsNullOrEmpty (tokenString));

            if (isRegexToken)
                tokenRegex = new Regex(tokenString, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            else
                this.tokenString = tokenString;
            this.tokenType = tokenType;
            this.availableInScopes = availableInScopes;
            this.scopeModifierFunc = scopeModifierFunc;
        }

        public bool IsRegexToken
        {
            get { return tokenRegex != null; }
        }

        public int TokenStringLength
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 1);
                return tokenString != null ? tokenString.Length : int.MaxValue;
            }
        }

        public string TokenString
        {
            get
            {
                if (IsRegexToken)
                    throw new InvalidOperationException("This is a regex token");

                return tokenString;
            }
        }

        public Regex TokenRegex
        {
            get
            {
                if (!IsRegexToken)
                    throw new InvalidOperationException ("This is not a regex token");

                return tokenRegex;
            }
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
        private readonly Regex tokenRegex;
        private readonly WikiTextToken.TokenType tokenType;
        private readonly WikiTextTokenScopes availableInScopes;
        private readonly Func<WikiTextTokenScopes, WikiTextTokenScopes> scopeModifierFunc;
    }
}