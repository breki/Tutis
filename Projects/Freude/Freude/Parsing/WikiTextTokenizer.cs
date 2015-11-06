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
            tokenDefinitions = PrepareTokens ();
        }

        public IList<WikiTextToken> TokenizeWikiText (string wikiText, WikiTokenizationSettings settings)
        {
            List<WikiTextToken> tokens = new List<WikiTextToken> ();

            int index = 0;
            int? textTokenStart = null;

            int scope = settings.IsWholeLine ? (int)WikiTextTokenScopes.LineStart : (int)WikiTextTokenScopes.InnerText;
            while (true)
            {
                if (index >= wikiText.Length)
                    break;

                int tokenEndingIndex;

                WikiTextTokenDef tokenFound = LookForMatchingToken (
                    wikiText, ref scope, index, out tokenEndingIndex);
                if (tokenFound != null)
                {
                    AddTextTokenIfAny (wikiText, tokens, ref textTokenStart, index, (WikiTextTokenScopes)scope);

                    string tokenText = wikiText.Substring(index, tokenEndingIndex - index);
                    tokens.Add (new WikiTextToken (tokenFound.TokenType, tokenText, (WikiTextTokenScopes)scope));
                    index = tokenEndingIndex;
                }
                else
                {
                    AddToTextToken (index, ref textTokenStart);
                    index++;
                }
            }

            AddTextTokenIfAny (wikiText, tokens, ref textTokenStart, index, (WikiTextTokenScopes)scope);

            return tokens;
        }

        private static void AddTextTokenIfAny (
            string wikiText, ICollection<WikiTextToken> tokens, ref int? textTokenStart, int index, WikiTextTokenScopes scope)
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
                    wikiText.Substring (textTokenStart.Value, index - textTokenStart.Value),
                    scope);

                tokens.Add (prevToken);
                textTokenStart = null;
            }
        }

        private WikiTextTokenDef LookForMatchingToken (
            string wikiText, ref int scope, int startingIndex, out int endingIndex)
        {
            Contract.Requires (wikiText != null);
            Contract.Requires (startingIndex >= 0 && startingIndex < wikiText.Length);
            Contract.Ensures (Contract.ValueAtReturn (out endingIndex) >= startingIndex
                && Contract.ValueAtReturn (out endingIndex) <= wikiText.Length);

            int charOffset = 0;
            int scopeCopied = scope;
            List<WikiTextTokenDef> partiallyMatchingTokens = new List<WikiTextTokenDef> (
                tokenDefinitions.Where (t => SelectTokensInScope(t, scopeCopied)));
            List<WikiTextTokenDef> matchedTokens = new List<WikiTextTokenDef> ();

            while (partiallyMatchingTokens.Count > 0)
            {
                if (startingIndex + charOffset >= wikiText.Length)
                    break;

                char textChar = wikiText[startingIndex + charOffset];

                int tokenIndex = 0;
                while (tokenIndex < partiallyMatchingTokens.Count)
                {
                    WikiTextTokenDef tokenDef = partiallyMatchingTokens[tokenIndex];

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
                WikiTextTokenDef longestMatchedToken = matchedTokens[matchedTokens.Count - 1];
                Contract.Assume (longestMatchedToken != null);
                endingIndex = startingIndex + longestMatchedToken.TokenString.Length;
                scope = longestMatchedToken.ModifyScope(scope) & ((int)~WikiTextTokenScopes.LineStart);
                return longestMatchedToken;
            }

            endingIndex = startingIndex;
            return null;
        }

        private static bool SelectTokensInScope (WikiTextTokenDef tokenDef, int scope)
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static List<WikiTextTokenDef> PrepareTokens ()
        {
            Contract.Ensures(Contract.Result<List<WikiTextTokenDef>>() != null);

            var def = new List<WikiTextTokenDef>
            {
                new WikiTextTokenDef("[", WikiTextToken.TokenType.SingleSquareBracketsOpen, WikiTextTokenScopes.LineStart | WikiTextTokenScopes.InnerText, x => WikiTextTokenScopes.LinkInternals),
                new WikiTextTokenDef("[[", WikiTextToken.TokenType.DoubleSquareBracketsOpen, WikiTextTokenScopes.LineStart | WikiTextTokenScopes.InnerText, x => WikiTextTokenScopes.LinkInternals),
                new WikiTextTokenDef("]", WikiTextToken.TokenType.SingleSquareBracketsClose, WikiTextTokenScopes.LineStart | WikiTextTokenScopes.InnerText | WikiTextTokenScopes.LinkInternals, x => WikiTextTokenScopes.InnerText),
                new WikiTextTokenDef("]]", WikiTextToken.TokenType.DoubleSquareBracketsClose, WikiTextTokenScopes.LineStart | WikiTextTokenScopes.InnerText | WikiTextTokenScopes.LinkInternals, x => WikiTextTokenScopes.InnerText),
                new WikiTextTokenDef("|", WikiTextToken.TokenType.Pipe, WikiTextTokenScopes.LinkInternals),
                new WikiTextTokenDef("''", WikiTextToken.TokenType.DoubleApostrophe, WikiTextTokenScopes.LineStart | WikiTextTokenScopes.InnerText | WikiTextTokenScopes.HeaderText, ModifyScopeForAnywhereTokens),
                new WikiTextTokenDef("'''", WikiTextToken.TokenType.TripleApostrophe, WikiTextTokenScopes.LineStart | WikiTextTokenScopes.InnerText | WikiTextTokenScopes.HeaderText, ModifyScopeForAnywhereTokens),
                new WikiTextTokenDef("=", WikiTextToken.TokenType.Header1Start, WikiTextTokenScopes.LineStart, x => x | WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("=", WikiTextToken.TokenType.Header1End, WikiTextTokenScopes.HeaderText, x => (x | WikiTextTokenScopes.HeaderSuffix) & ~WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("==", WikiTextToken.TokenType.Header2Start, WikiTextTokenScopes.LineStart, x => x | WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("==", WikiTextToken.TokenType.Header2End, WikiTextTokenScopes.HeaderText, x => (x | WikiTextTokenScopes.HeaderSuffix) & ~WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("===", WikiTextToken.TokenType.Header3Start, WikiTextTokenScopes.LineStart, x => x | WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("===", WikiTextToken.TokenType.Header3End, WikiTextTokenScopes.HeaderText, x => (x | WikiTextTokenScopes.HeaderSuffix) & ~WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("====", WikiTextToken.TokenType.Header4Start, WikiTextTokenScopes.LineStart, x => x | WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("====", WikiTextToken.TokenType.Header4End, WikiTextTokenScopes.HeaderText, x => (x | WikiTextTokenScopes.HeaderSuffix) & ~WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("=====", WikiTextToken.TokenType.Header5Start, WikiTextTokenScopes.LineStart, x => x | WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("=====", WikiTextToken.TokenType.Header5End, WikiTextTokenScopes.HeaderText, x => (x | WikiTextTokenScopes.HeaderSuffix) & ~WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("======", WikiTextToken.TokenType.Header6Start, WikiTextTokenScopes.LineStart, x => x | WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("======", WikiTextToken.TokenType.Header6End, WikiTextTokenScopes.HeaderText, x => (x | WikiTextTokenScopes.HeaderSuffix) & ~WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("#", WikiTextToken.TokenType.HeaderAnchor, WikiTextTokenScopes.HeaderSuffix),
                new WikiTextTokenDef("*", WikiTextToken.TokenType.BulletList, WikiTextTokenScopes.LineStart),
                new WikiTextTokenDef("#", WikiTextToken.TokenType.NumberedList, WikiTextTokenScopes.LineStart)
            };

            def.Sort ((a, b) => -a.TokenString.Length.CompareTo (b.TokenString.Length));
            return def;
        }

        private static WikiTextTokenScopes ModifyScopeForAnywhereTokens (WikiTextTokenScopes scope)
        {
            return scope == WikiTextTokenScopes.LineStart ? WikiTextTokenScopes.InnerText : scope;
        }

        [ContractInvariantMethod]
        private void Invariant ()
        {
            Contract.Invariant (Contract.ForAll (tokenDefinitions, x => x != null));
        }

        private readonly List<WikiTextTokenDef> tokenDefinitions;
    }
}