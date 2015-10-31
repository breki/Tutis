using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Freude.Parsing
{
    public class WikiTextTokenizer : IWikiTextTokenizer
    {
        public WikiTextTokenizer()
        {
            tokenDefinitions.Add(new TokenDef("[[", WikiTextToken.TokenType.DoubleSquareBracketsOpen));
            tokenDefinitions.Add(new TokenDef("]]", WikiTextToken.TokenType.DoubleSquareBracketsClose));
            tokenDefinitions.Sort((a, b) => -a.TokenString.Length.CompareTo(b.TokenString.Length));
        }

        public IList<WikiTextToken> TokenizeWikiText(string wikiText)
        {
            List<WikiTextToken> tokens = new List<WikiTextToken>();

            int index = 0;
            int? textTokenStart = 0;

            while (true)
            {
                if (index >= wikiText.Length)
                    break;

                int tokenEndingIndex;

                WikiTextToken.TokenType? foundType = LookForMatchingToken(wikiText, index, out tokenEndingIndex);
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
            string wikiText, int startingIndex, out int endingIndex)
        {
            Contract.Requires(wikiText != null);
            Contract.Requires (startingIndex >= 0 && startingIndex < wikiText.Length);
            Contract.Ensures (Contract.ValueAtReturn (out endingIndex) >= startingIndex 
                && Contract.ValueAtReturn (out endingIndex) <= wikiText.Length);

            int charOffset = 0;
            List<TokenDef> partiallyMatchingTokens = new List<TokenDef> (tokenDefinitions);
            List<TokenDef> matchedTokens = new List<TokenDef> ();
            
            while (partiallyMatchingTokens.Count > 0)
            {
                if (startingIndex + charOffset >= wikiText.Length)
                    throw new NotImplementedException("todo next: reached the end of text");

                char textChar = wikiText[startingIndex + charOffset];

                int tokenIndex = 0;
                while (tokenIndex < partiallyMatchingTokens.Count)
                {
                    TokenDef tokenDef = partiallyMatchingTokens[tokenIndex];
                    if (charOffset >= tokenDef.TokenString.Length)
                    {
                        matchedTokens.Add(tokenDef);
                        partiallyMatchingTokens.RemoveAt (tokenIndex);
                        continue;
                    }

                    char tokenChar = tokenDef.TokenString[charOffset];
                    if (tokenChar == textChar)
                        tokenIndex++;
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

        private static void AddToTextToken(
            int index, 
            ref int? textTokenStart)
        {
            Contract.Requires(index >= 0);
            Contract.Ensures (textTokenStart != null);

            if (!textTokenStart.HasValue)
                textTokenStart = index;
        }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(Contract.ForAll(tokenDefinitions, x => x != null));
        }

        private readonly List<TokenDef> tokenDefinitions = new List<TokenDef>();

        private class TokenDef
        {
            public TokenDef(string tokenString, WikiTextToken.TokenType tokenType)
            {
                this.tokenString = tokenString;
                this.tokenType = tokenType;
            }

            public string TokenString
            {
                get { return tokenString; }
            }

            public WikiTextToken.TokenType TokenType
            {
                get { return tokenType; }
            }

            private readonly string tokenString;
            private readonly WikiTextToken.TokenType tokenType;
        }
    }
}