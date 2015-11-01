﻿using System;
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

            int scope = settings.IsWholeLine ? (int)TokenScopes.LineStart : (int)TokenScopes.InnerText;
            while (true)
            {
                if (index >= wikiText.Length)
                    break;

                int tokenEndingIndex;

                TokenDef tokenFound = LookForMatchingToken (
                    wikiText, ref scope, index, out tokenEndingIndex);
                if (tokenFound != null)
                {
                    AddTextTokenIfAny (wikiText, tokens, ref textTokenStart, index);

                    string tokenText = wikiText.Substring(index, tokenEndingIndex - index);
                    tokens.Add (new WikiTextToken (tokenFound.TokenType, tokenText));
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
                    wikiText.Substring (textTokenStart.Value, index - textTokenStart.Value));

                tokens.Add (prevToken);
                textTokenStart = null;
            }
        }

        private TokenDef LookForMatchingToken (
            string wikiText, ref int scope, int startingIndex, out int endingIndex)
        {
            Contract.Requires (wikiText != null);
            Contract.Requires (startingIndex >= 0 && startingIndex < wikiText.Length);
            Contract.Ensures (Contract.ValueAtReturn (out endingIndex) >= startingIndex
                && Contract.ValueAtReturn (out endingIndex) <= wikiText.Length);

            int charOffset = 0;
            int scopeCopied = scope;
            List<TokenDef> partiallyMatchingTokens = new List<TokenDef> (
                tokenDefinitions.Where (t => SelectTokensInScope(t, scopeCopied)));
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
                scope = longestMatchedToken.ModifyScope(scope) & ((int)~TokenScopes.LineStart);
                return longestMatchedToken;
            }

            endingIndex = startingIndex;
            return null;
        }

        private static bool SelectTokensInScope (TokenDef tokenDef, int scope)
        {
            return ((int)tokenDef.AvailableInScopes & scope) != 0;
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
            tokenDefinitions.Add (new TokenDef ("[", WikiTextToken.TokenType.SingleSquareBracketsOpen, TokenScopes.LineStart | TokenScopes.InnerText, TokenScopes.None, TokenScopes.None));
            tokenDefinitions.Add (new TokenDef ("[[", WikiTextToken.TokenType.DoubleSquareBracketsOpen, TokenScopes.LineStart | TokenScopes.InnerText, TokenScopes.None, TokenScopes.None));
            tokenDefinitions.Add (new TokenDef ("]", WikiTextToken.TokenType.SingleSquareBracketsClose, TokenScopes.LineStart | TokenScopes.InnerText, TokenScopes.None, TokenScopes.None));
            tokenDefinitions.Add (new TokenDef ("]]", WikiTextToken.TokenType.DoubleSquareBracketsClose, TokenScopes.LineStart | TokenScopes.InnerText, TokenScopes.None, TokenScopes.None));
            tokenDefinitions.Add (new TokenDef ("|", WikiTextToken.TokenType.Pipe, TokenScopes.LineStart | TokenScopes.InnerText, TokenScopes.None, TokenScopes.None));
            tokenDefinitions.Add (new TokenDef ("''", WikiTextToken.TokenType.DoubleApostrophe, TokenScopes.LineStart | TokenScopes.InnerText | TokenScopes.HeaderText, TokenScopes.None, TokenScopes.None));
            tokenDefinitions.Add (new TokenDef ("'''", WikiTextToken.TokenType.TripleApostrophe, TokenScopes.LineStart | TokenScopes.InnerText | TokenScopes.HeaderText, TokenScopes.None, TokenScopes.None));
            tokenDefinitions.Add (new TokenDef ("=", WikiTextToken.TokenType.Header1Start, TokenScopes.LineStart, TokenScopes.HeaderText, TokenScopes.None));
            tokenDefinitions.Add (new TokenDef ("=", WikiTextToken.TokenType.Header1End, TokenScopes.HeaderText, TokenScopes.HeaderText, TokenScopes.HeaderSuffix));
            tokenDefinitions.Add (new TokenDef ("==", WikiTextToken.TokenType.Header2Start, TokenScopes.LineStart, TokenScopes.HeaderText, TokenScopes.None));
            tokenDefinitions.Add (new TokenDef ("==", WikiTextToken.TokenType.Header2End, TokenScopes.HeaderText, TokenScopes.HeaderText, TokenScopes.HeaderSuffix));
            tokenDefinitions.Add (new TokenDef ("===", WikiTextToken.TokenType.Header3Start, TokenScopes.LineStart, TokenScopes.HeaderText, TokenScopes.None));
            tokenDefinitions.Add (new TokenDef ("===", WikiTextToken.TokenType.Header3End, TokenScopes.HeaderText, TokenScopes.HeaderText, TokenScopes.HeaderSuffix));
            tokenDefinitions.Add (new TokenDef ("====", WikiTextToken.TokenType.Header4Start, TokenScopes.LineStart, TokenScopes.HeaderText, TokenScopes.None));
            tokenDefinitions.Add (new TokenDef ("====", WikiTextToken.TokenType.Header4End, TokenScopes.HeaderText, TokenScopes.HeaderText, TokenScopes.HeaderSuffix));
            tokenDefinitions.Add (new TokenDef ("=====", WikiTextToken.TokenType.Header5Start, TokenScopes.LineStart, TokenScopes.HeaderText, TokenScopes.None));
            tokenDefinitions.Add (new TokenDef ("=====", WikiTextToken.TokenType.Header5End, TokenScopes.HeaderText, TokenScopes.HeaderText, TokenScopes.HeaderSuffix));
            tokenDefinitions.Add (new TokenDef ("======", WikiTextToken.TokenType.Header6Start, TokenScopes.LineStart, TokenScopes.HeaderText, TokenScopes.None));
            tokenDefinitions.Add (new TokenDef ("======", WikiTextToken.TokenType.Header6End, TokenScopes.HeaderText, TokenScopes.HeaderText, TokenScopes.HeaderSuffix));
            tokenDefinitions.Add (new TokenDef ("#", WikiTextToken.TokenType.HeaderAnchor, TokenScopes.HeaderSuffix, TokenScopes.None, TokenScopes.None));
            tokenDefinitions.Add (new TokenDef ("*", WikiTextToken.TokenType.BulletList, TokenScopes.LineStart, TokenScopes.None, TokenScopes.None));
            tokenDefinitions.Add (new TokenDef ("#", WikiTextToken.TokenType.NumberedList, TokenScopes.LineStart, TokenScopes.None, TokenScopes.None));
            tokenDefinitions.Sort ((a, b) => -a.TokenString.Length.CompareTo (b.TokenString.Length));
        }

        [ContractInvariantMethod]
        private void Invariant ()
        {
            Contract.Invariant (Contract.ForAll (tokenDefinitions, x => x != null));
        }

        private readonly List<TokenDef> tokenDefinitions = new List<TokenDef> ();

        [Flags]
        public enum TokenScopes
        {
            None = 0,
            LineStart = 1 << 0,
            InnerText = 1 << 1,
            HeaderText = 1 << 2,
            HeaderSuffix = 1 << 3,
        }

        private class TokenDef
        {
            public TokenDef (
                string tokenString, 
                WikiTextToken.TokenType tokenType, 
                TokenScopes availableInScopes,
                TokenScopes beginsScopes,
                TokenScopes endsScopes)
            {
                Contract.Requires (!string.IsNullOrEmpty (tokenString));

                this.tokenString = tokenString;
                this.tokenType = tokenType;
                this.availableInScopes = availableInScopes;
                this.beginsScopes = beginsScopes;
                this.endsScopes = endsScopes;
            }

            public string TokenString
            {
                get { return tokenString; }
            }

            public WikiTextToken.TokenType TokenType
            {
                get { return tokenType; }
            }

            public TokenScopes AvailableInScopes
            {
                get { return availableInScopes; }
            }

            public int ModifyScope(int currentScope)
            {
                return currentScope | (int)(beginsScopes & ~endsScopes);
            }

            private readonly string tokenString;
            private readonly WikiTextToken.TokenType tokenType;
            private readonly TokenScopes availableInScopes;
            private readonly TokenScopes beginsScopes;
            private readonly TokenScopes endsScopes;
        }
    }
}