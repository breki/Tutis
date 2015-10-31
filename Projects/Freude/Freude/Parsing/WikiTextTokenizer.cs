using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Freude.Parsing
{
    public class WikiTextTokenizer : IWikiTextTokenizer
    {
        public WikiTextTokenizer()
        {
            PrepareTokens();
        }

        public IList<WikiTextToken> TokenizeWikiText(string wikiText, WikiTokenizationSettings settings)
        {
            List<WikiTextToken> tokens = new List<WikiTextToken>();

            int index = 0;
            int? textTokenStart = null;

            while (true)
            {
                if (index >= wikiText.Length)
                    break;

                int tokenEndingIndex;

                WikiTextToken.TokenType? foundType = LookForMatchingToken(
                    wikiText, settings.IsWholeLine, index, out tokenEndingIndex);
                if (foundType.HasValue)
                {
                    AddTextTokenIfAny(wikiText, tokens, ref textTokenStart, index);

                    tokens.Add(new WikiTextToken(foundType.Value));
                    index = tokenEndingIndex;
                }
                else
                {
                    AddToTextToken (index, ref textTokenStart);
                    index++;
                }
            }

            AddTextTokenIfAny (wikiText, tokens, ref textTokenStart, index);

            return tokens;
        }

        private static void AddTextTokenIfAny(string wikiText, ICollection<WikiTextToken> tokens, ref int? textTokenStart, int index)
        {
            Contract.Requires(wikiText != null);
            Contract.Requires(tokens != null);
            Contract.Requires(textTokenStart == null 
                || (textTokenStart != null 
                && index > textTokenStart.Value 
                && textTokenStart.Value < wikiText.Length
                && index <= wikiText.Length));
            Contract.Ensures(textTokenStart == null);

            if (textTokenStart.HasValue)
            {
                WikiTextToken prevToken = new WikiTextToken(
                    WikiTextToken.TokenType.Text,
                    wikiText.Substring(textTokenStart.Value, index - textTokenStart.Value));

                tokens.Add(prevToken);
                textTokenStart = null;
            }
        }

        private WikiTextToken.TokenType? LookForMatchingToken(
            string wikiText, bool isWholeLine, int startingIndex, out int endingIndex)
        {
            Contract.Requires(wikiText != null);
            Contract.Requires (startingIndex >= 0 && startingIndex < wikiText.Length);
            Contract.Ensures (Contract.ValueAtReturn (out endingIndex) >= startingIndex 
                && Contract.ValueAtReturn (out endingIndex) <= wikiText.Length);

            bool isBeginningOfLine = isWholeLine && startingIndex == 0;

            int charOffset = 0;
            List<TokenDef> partiallyMatchingTokens = new List<TokenDef> (
                tokenDefinitions.Where(t => SelectTokensInScope(t, isBeginningOfLine)));
            List<TokenDef> matchedTokens = new List<TokenDef> ();
            
            while (partiallyMatchingTokens.Count > 0)
            {
                if (startingIndex + charOffset >= wikiText.Length)
                    break;

                char textChar = wikiText[startingIndex + charOffset];

                int tokenIndex = 0;
                while (tokenIndex < partiallyMatchingTokens.Count)
                {
                    TokenDef tokenDef = partiallyMatchingTokens[tokenIndex];

                    char tokenChar = tokenDef.TokenString[charOffset];
                    if (tokenChar == textChar)
                    {
                        if (charOffset == tokenDef.TokenString.Length - 1)
                        {
                            matchedTokens.Add (tokenDef);
                            partiallyMatchingTokens.RemoveAt (tokenIndex);
                            continue;
                        }

                        tokenIndex++;
                    }
                    else
                        partiallyMatchingTokens.RemoveAt(tokenIndex);
                }

                charOffset++;
            }

            if (matchedTokens.Count > 0)
            {
                TokenDef longestMatchedToken = matchedTokens[matchedTokens.Count - 1];
                Contract.Assume(longestMatchedToken != null);
                endingIndex = startingIndex + longestMatchedToken.TokenString.Length;
                return longestMatchedToken.TokenType;
            }

            endingIndex = startingIndex;
            return null;
        }

        private static bool SelectTokensInScope(TokenDef tokenDef, bool isBeginningOfLine)
        {
            switch (tokenDef.Scope)
            {
                case TokenDef.TokenScope.Anywhere:
                    return true;
                case TokenDef.TokenScope.BeginLineOnly:
                    return isBeginningOfLine;
                case TokenDef.TokenScope.NotAtBeginLine:
                    return !isBeginningOfLine;
                default:
                    throw new NotSupportedException();
            }
        }

        private static void AddToTextToken(
            int index, 
            ref int? textTokenStart)
        {
            Contract.Requires(index >= 0);
            Contract.Ensures (textTokenStart != null);

            if (!textTokenStart.HasValue)
                textTokenStart = index;
        }

        private void PrepareTokens()
        {
            tokenDefinitions.Add(new TokenDef("[", WikiTextToken.TokenType.SingleSquareBracketsOpen));
            tokenDefinitions.Add(new TokenDef("[[", WikiTextToken.TokenType.DoubleSquareBracketsOpen));
            tokenDefinitions.Add(new TokenDef("]", WikiTextToken.TokenType.SingleSquareBracketsClose));
            tokenDefinitions.Add(new TokenDef("]]", WikiTextToken.TokenType.DoubleSquareBracketsClose));
            tokenDefinitions.Add(new TokenDef("|", WikiTextToken.TokenType.Pipe));
            tokenDefinitions.Add(new TokenDef("''", WikiTextToken.TokenType.DoubleApostrophe));
            tokenDefinitions.Add(new TokenDef("'''", WikiTextToken.TokenType.TripleApostrophe));
            tokenDefinitions.Add(new TokenDef("==", WikiTextToken.TokenType.Header2Start, TokenDef.TokenScope.BeginLineOnly));
            tokenDefinitions.Add(new TokenDef("==", WikiTextToken.TokenType.Header2End, TokenDef.TokenScope.NotAtBeginLine));
            tokenDefinitions.Add(new TokenDef("===", WikiTextToken.TokenType.Header3Start, TokenDef.TokenScope.BeginLineOnly));
            tokenDefinitions.Add(new TokenDef("===", WikiTextToken.TokenType.Header3End, TokenDef.TokenScope.NotAtBeginLine));
            tokenDefinitions.Add(new TokenDef("====", WikiTextToken.TokenType.Header4Start, TokenDef.TokenScope.BeginLineOnly));
            tokenDefinitions.Add(new TokenDef("====", WikiTextToken.TokenType.Header4End, TokenDef.TokenScope.NotAtBeginLine));
            tokenDefinitions.Add(new TokenDef("=====", WikiTextToken.TokenType.Header5Start, TokenDef.TokenScope.BeginLineOnly));
            tokenDefinitions.Add(new TokenDef("=====", WikiTextToken.TokenType.Header5End, TokenDef.TokenScope.NotAtBeginLine));
            tokenDefinitions.Add(new TokenDef("======", WikiTextToken.TokenType.Header6Start, TokenDef.TokenScope.BeginLineOnly));
            tokenDefinitions.Add(new TokenDef("======", WikiTextToken.TokenType.Header6End, TokenDef.TokenScope.NotAtBeginLine));
            tokenDefinitions.Add(new TokenDef("*", WikiTextToken.TokenType.BulletList, TokenDef.TokenScope.BeginLineOnly));
            tokenDefinitions.Add(new TokenDef("#", WikiTextToken.TokenType.NumberedList, TokenDef.TokenScope.BeginLineOnly));
            tokenDefinitions.Sort((a, b) => -a.TokenString.Length.CompareTo(b.TokenString.Length));
        }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(Contract.ForAll(tokenDefinitions, x => x != null));
        }

        private readonly List<TokenDef> tokenDefinitions = new List<TokenDef>();

        private class TokenDef
        {
            public TokenDef(string tokenString, WikiTextToken.TokenType tokenType, TokenScope scope = TokenScope.Anywhere)
            {
                Contract.Requires(!string.IsNullOrEmpty(tokenString));

                this.tokenString = tokenString;
                this.tokenType = tokenType;
                this.scope = scope;
            }

            public string TokenString
            {
                get { return tokenString; }
            }

            public WikiTextToken.TokenType TokenType
            {
                get { return tokenType; }
            }

            public TokenScope Scope
            {
                get { return scope; }
            }

            private readonly string tokenString;
            private readonly WikiTextToken.TokenType tokenType;
            private readonly TokenScope scope;

            public enum TokenScope
            {
                Anywhere,
                BeginLineOnly,
                NotAtBeginLine
            }
        }
    }
}