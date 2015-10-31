using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Freude.Parsing
{
    public class WikiTextTokenizer : IWikiTextTokenizer
    {
        public WikiTextTokenizer ()
        {
            PrepareTokens ();
        }

        public IList<WikiTextToken> TokenizeWikiText (string wikiText, WikiTokenizationSettings settings)
        {
            List<WikiTextToken> tokens = new List<WikiTextToken> ();

            int index = 0;
            int? textTokenStart = null;

            while (true)
            {
                if (index >= wikiText.Length)
                    break;

                int tokenEndingIndex;

                TokenDef tokenFound = LookForMatchingToken (
                    wikiText, settings.IsWholeLine, index, out tokenEndingIndex);
                if (tokenFound != null)
                {
                    AddTextTokenIfAny (wikiText, tokens, ref textTokenStart, index);

                    string tokenText = wikiText.Substring(index, tokenEndingIndex - index);
                    tokens.Add (new WikiTextToken (tokenFound.TokenType, tokenFound.Scope, tokenText));
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

        private static void AddTextTokenIfAny (string wikiText, ICollection<WikiTextToken> tokens, ref int? textTokenStart, int index)
        {
            Contract.Requires (wikiText != null);
            Contract.Requires (tokens != null);
            Contract.Requires (textTokenStart == null
                || (textTokenStart != null
                && index > textTokenStart.Value
                && textTokenStart.Value < wikiText.Length
                && index <= wikiText.Length));
            Contract.Ensures (textTokenStart == null);

            if (textTokenStart.HasValue)
            {
                WikiTextToken prevToken = new WikiTextToken (
                    WikiTextToken.TokenType.Text,
                    WikiTextToken.TokenScope.Anywhere,
                    wikiText.Substring (textTokenStart.Value, index - textTokenStart.Value));

                tokens.Add (prevToken);
                textTokenStart = null;
            }
        }

        private TokenDef LookForMatchingToken (
            string wikiText, bool isWholeLine, int startingIndex, out int endingIndex)
        {
            Contract.Requires (wikiText != null);
            Contract.Requires (startingIndex >= 0 && startingIndex < wikiText.Length);
            Contract.Ensures (Contract.ValueAtReturn (out endingIndex) >= startingIndex
                && Contract.ValueAtReturn (out endingIndex) <= wikiText.Length);

            bool isBeginningOfLine = isWholeLine && startingIndex == 0;

            int charOffset = 0;
            List<TokenDef> partiallyMatchingTokens = new List<TokenDef> (
                tokenDefinitions.Where (t => SelectTokensInScope (t, isBeginningOfLine)));
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
                        partiallyMatchingTokens.RemoveAt (tokenIndex);
                }

                charOffset++;
            }

            if (matchedTokens.Count > 0)
            {
                TokenDef longestMatchedToken = matchedTokens[matchedTokens.Count - 1];
                Contract.Assume (longestMatchedToken != null);
                endingIndex = startingIndex + longestMatchedToken.TokenString.Length;
                return longestMatchedToken;
            }

            endingIndex = startingIndex;
            return null;
        }

        private static bool SelectTokensInScope (TokenDef tokenDef, bool isBeginningOfLine)
        {
            switch (tokenDef.Scope)
            {
                case WikiTextToken.TokenScope.Anywhere:
                    return true;
                case WikiTextToken.TokenScope.BeginLineOnly:
                    return isBeginningOfLine;
                case WikiTextToken.TokenScope.NotAtBeginLine:
                    return !isBeginningOfLine;
                default:
                    throw new NotSupportedException ();
            }
        }

        private static void AddToTextToken (
            int index,
            ref int? textTokenStart)
        {
            Contract.Requires (index >= 0);
            Contract.Ensures (textTokenStart != null);

            if (!textTokenStart.HasValue)
                textTokenStart = index;
        }

        private void PrepareTokens ()
        {
            tokenDefinitions.Add (new TokenDef ("[", WikiTextToken.TokenType.SingleSquareBracketsOpen));
            tokenDefinitions.Add (new TokenDef ("[[", WikiTextToken.TokenType.DoubleSquareBracketsOpen));
            tokenDefinitions.Add (new TokenDef ("]", WikiTextToken.TokenType.SingleSquareBracketsClose));
            tokenDefinitions.Add (new TokenDef ("]]", WikiTextToken.TokenType.DoubleSquareBracketsClose));
            tokenDefinitions.Add (new TokenDef ("|", WikiTextToken.TokenType.Pipe));
            tokenDefinitions.Add (new TokenDef ("''", WikiTextToken.TokenType.DoubleApostrophe));
            tokenDefinitions.Add (new TokenDef ("'''", WikiTextToken.TokenType.TripleApostrophe));
            tokenDefinitions.Add (new TokenDef ("=", WikiTextToken.TokenType.Header1Start, WikiTextToken.TokenScope.BeginLineOnly));
            tokenDefinitions.Add (new TokenDef ("=", WikiTextToken.TokenType.Header1End, WikiTextToken.TokenScope.NotAtBeginLine));
            tokenDefinitions.Add (new TokenDef ("==", WikiTextToken.TokenType.Header2Start, WikiTextToken.TokenScope.BeginLineOnly));
            tokenDefinitions.Add (new TokenDef ("==", WikiTextToken.TokenType.Header2End, WikiTextToken.TokenScope.NotAtBeginLine));
            tokenDefinitions.Add (new TokenDef ("===", WikiTextToken.TokenType.Header3Start, WikiTextToken.TokenScope.BeginLineOnly));
            tokenDefinitions.Add (new TokenDef ("===", WikiTextToken.TokenType.Header3End, WikiTextToken.TokenScope.NotAtBeginLine));
            tokenDefinitions.Add (new TokenDef ("====", WikiTextToken.TokenType.Header4Start, WikiTextToken.TokenScope.BeginLineOnly));
            tokenDefinitions.Add (new TokenDef ("====", WikiTextToken.TokenType.Header4End, WikiTextToken.TokenScope.NotAtBeginLine));
            tokenDefinitions.Add (new TokenDef ("=====", WikiTextToken.TokenType.Header5Start, WikiTextToken.TokenScope.BeginLineOnly));
            tokenDefinitions.Add (new TokenDef ("=====", WikiTextToken.TokenType.Header5End, WikiTextToken.TokenScope.NotAtBeginLine));
            tokenDefinitions.Add (new TokenDef ("======", WikiTextToken.TokenType.Header6Start, WikiTextToken.TokenScope.BeginLineOnly));
            tokenDefinitions.Add (new TokenDef ("======", WikiTextToken.TokenType.Header6End, WikiTextToken.TokenScope.NotAtBeginLine));
            tokenDefinitions.Add (new TokenDef ("*", WikiTextToken.TokenType.BulletList, WikiTextToken.TokenScope.BeginLineOnly));
            tokenDefinitions.Add (new TokenDef ("#", WikiTextToken.TokenType.NumberedList, WikiTextToken.TokenScope.BeginLineOnly));
            tokenDefinitions.Sort ((a, b) => -a.TokenString.Length.CompareTo (b.TokenString.Length));
        }

        [ContractInvariantMethod]
        private void Invariant ()
        {
            Contract.Invariant (Contract.ForAll (tokenDefinitions, x => x != null));
        }

        private readonly List<TokenDef> tokenDefinitions = new List<TokenDef> ();

        private class TokenDef
        {
            public TokenDef (string tokenString, WikiTextToken.TokenType tokenType, WikiTextToken.TokenScope scope = WikiTextToken.TokenScope.Anywhere)
            {
                Contract.Requires (!string.IsNullOrEmpty (tokenString));

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

            public WikiTextToken.TokenScope Scope
            {
                get { return scope; }
            }

            private readonly string tokenString;
            private readonly WikiTextToken.TokenType tokenType;
            private readonly WikiTextToken.TokenScope scope;
        }
    }
}