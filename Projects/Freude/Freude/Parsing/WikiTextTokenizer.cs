using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
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

            if (!textTokenStart.HasValue) 
                return;

            WikiTextToken prevToken = new WikiTextToken (
                WikiTextToken.TokenType.Text,
                wikiText.Substring (textTokenStart.Value, index - textTokenStart.Value),
                scope);

            tokens.Add (prevToken);
            textTokenStart = null;
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
                bool shouldContinue = HandleNextChar(wikiText, startingIndex, ref charOffset, partiallyMatchingTokens, matchedTokens);
                if (!shouldContinue)
                    break;
            }

            // if there are no matching tokens...
            if (matchedTokens.Count == 0)
            {
                endingIndex = startingIndex;
                return null;
            }

            return FetchBestMatchingToken(ref scope, startingIndex, out endingIndex, matchedTokens, charOffset);
        }

        private static bool HandleNextChar(
            string wikiText, 
            int startingIndex, 
            ref int charOffset, 
            IList<WikiTextTokenDef> partiallyMatchingTokens, 
            ICollection<WikiTextTokenDef> matchedTokens)
        {
            Contract.Requires(charOffset >= 0);
            Contract.Ensures(charOffset >= 0);

            if (IsEndOfText(wikiText, startingIndex, charOffset, partiallyMatchingTokens, matchedTokens)) 
                return false;
            
            char textChar = wikiText[startingIndex + charOffset];

            int tokenIndex = 0;
            while (tokenIndex < partiallyMatchingTokens.Count)
            {
                MatchCharToToken(partiallyMatchingTokens, matchedTokens, ref tokenIndex, charOffset, textChar);
            }

            charOffset++;
            return true;
        }

        private static void MatchCharToToken(
            IList<WikiTextTokenDef> partiallyMatchingTokens, 
            ICollection<WikiTextTokenDef> matchedTokens, 
            ref int tokenIndex, 
            int charOffset, 
            char textChar)
        {
            Contract.Requires(tokenIndex >= 0 && tokenIndex < partiallyMatchingTokens.Count);
            Contract.Ensures(tokenIndex >= 0 && tokenIndex < partiallyMatchingTokens.Count);

            WikiTextTokenDef tokenDef = partiallyMatchingTokens[tokenIndex];

            bool tokenStillMatches;
            bool tokenAlreadyRemoved = DetermineIfTokenStillMatches(
                partiallyMatchingTokens, matchedTokens, tokenIndex, tokenDef, out tokenStillMatches, charOffset, textChar);
            if (tokenAlreadyRemoved) 
                return;

            HandleMatchingOrNonMatching(partiallyMatchingTokens, matchedTokens, ref tokenIndex, tokenDef, tokenStillMatches, charOffset);
        }

        private static bool DetermineIfTokenStillMatches(
            IList<WikiTextTokenDef> partiallyMatchingTokens, 
            ICollection<WikiTextTokenDef> matchedTokens, 
            int tokenIndex, 
            WikiTextTokenDef tokenDef, 
            out bool tokenStillMatches, 
            int charOffset, 
            char textChar)
        {
            Contract.Requires(partiallyMatchingTokens != null);
            Contract.Requires(matchedTokens != null);
            Contract.Requires(tokenDef != null);

            if (tokenDef.IsRegexToken)
                tokenStillMatches = tokenDef.TokenRegex.IsMatch(textChar.ToString(CultureInfo.InvariantCulture));
            else
            {
                char tokenChar = tokenDef.TokenString[charOffset];
                if (tokenChar == textChar)
                {
                    if (charOffset == tokenDef.TokenString.Length - 1)
                    {
                        matchedTokens.Add(tokenDef);
                        partiallyMatchingTokens.RemoveAt(tokenIndex);
                        tokenStillMatches = false;
                        return true;
                    }

                    tokenStillMatches = true;
                }
                else
                    tokenStillMatches = false;
            }

            return false;
        }

        private static void HandleMatchingOrNonMatching(
            IList<WikiTextTokenDef> partiallyMatchingTokens, 
            ICollection<WikiTextTokenDef> matchedTokens, 
            ref int tokenIndex, 
            WikiTextTokenDef tokenDef, 
            bool tokenStillMatches, 
            int charOffset)
        {
            if (tokenStillMatches)
                tokenIndex++;
            else
            {
                if (tokenDef.IsRegexToken && charOffset > 0)
                    matchedTokens.Add(tokenDef);

                partiallyMatchingTokens.RemoveAt(tokenIndex);
            }
        }

        private static bool IsEndOfText(
            string wikiText, 
            int startingIndex, 
            int charOffset, 
            IList<WikiTextTokenDef> partiallyMatchingTokens, 
            ICollection<WikiTextTokenDef> matchedTokens)
        {
            Contract.Requires(wikiText != null);
            Contract.Requires(partiallyMatchingTokens != null);
            Contract.Requires(matchedTokens != null);

            if (startingIndex + charOffset < wikiText.Length) 
                return false;

            if (partiallyMatchingTokens.Count == 0) 
                return true;

            WikiTextTokenDef tokenDef = partiallyMatchingTokens[0];
            if (tokenDef.IsRegexToken && charOffset > 0)
                matchedTokens.Add(tokenDef);

            return true;
        }

        private static WikiTextTokenDef FetchBestMatchingToken(
            ref int scope, int startingIndex, out int endingIndex, IList<WikiTextTokenDef> matchedTokens, int charOffset)
        {
            WikiTextTokenDef longestMatchedToken = matchedTokens[matchedTokens.Count - 1];
            Contract.Assume(longestMatchedToken != null);

            if (longestMatchedToken.IsRegexToken)
                endingIndex = startingIndex + charOffset - 1;
            else
                endingIndex = startingIndex + longestMatchedToken.TokenStringLength;

            scope = longestMatchedToken.ModifyScope(scope) & ((int)~WikiTextTokenScopes.LineStart);
            return longestMatchedToken;
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

        [SuppressMessage ("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private static List<WikiTextTokenDef> PrepareTokens ()
        {
            Contract.Ensures(Contract.Result<List<WikiTextTokenDef>>() != null);

            var def = new List<WikiTextTokenDef>
            {
                new WikiTextTokenDef("[", false, WikiTextToken.TokenType.SingleSquareBracketsOpen, WikiTextTokenScopes.LineStart | WikiTextTokenScopes.InnerText, x => WikiTextTokenScopes.ExternalLinkUrl),
                new WikiTextTokenDef("[[", false, WikiTextToken.TokenType.DoubleSquareBracketsOpen, WikiTextTokenScopes.LineStart | WikiTextTokenScopes.InnerText, x => WikiTextTokenScopes.InternalLinkInternals),
                new WikiTextTokenDef("]", false, WikiTextToken.TokenType.SingleSquareBracketsClose, WikiTextTokenScopes.LineStart | WikiTextTokenScopes.InnerText | WikiTextTokenScopes.ExternalLinkUrl | WikiTextTokenScopes.ExternalLinkText, x => WikiTextTokenScopes.InnerText),
                new WikiTextTokenDef("]]", false, WikiTextToken.TokenType.DoubleSquareBracketsClose, WikiTextTokenScopes.LineStart | WikiTextTokenScopes.InnerText | WikiTextTokenScopes.InternalLinkInternals, x => WikiTextTokenScopes.InnerText),
                new WikiTextTokenDef(":", false, WikiTextToken.TokenType.NamespaceSeparator, WikiTextTokenScopes.InternalLinkInternals),
                new WikiTextTokenDef("|", false, WikiTextToken.TokenType.Pipe, WikiTextTokenScopes.InternalLinkInternals),
                new WikiTextTokenDef("''", false, WikiTextToken.TokenType.DoubleApostrophe, WikiTextTokenScopes.LineStart | WikiTextTokenScopes.InnerText | WikiTextTokenScopes.HeaderText | WikiTextTokenScopes.ExternalLinkText, ModifyScopeForAnywhereTokens),
                new WikiTextTokenDef("'''", false, WikiTextToken.TokenType.TripleApostrophe, WikiTextTokenScopes.LineStart | WikiTextTokenScopes.InnerText | WikiTextTokenScopes.HeaderText | WikiTextTokenScopes.ExternalLinkText, ModifyScopeForAnywhereTokens),
                new WikiTextTokenDef("=", false, WikiTextToken.TokenType.Header1Start, WikiTextTokenScopes.LineStart, x => x | WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("=", false, WikiTextToken.TokenType.Header1End, WikiTextTokenScopes.HeaderText, x => (x | WikiTextTokenScopes.HeaderSuffix) & ~WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("==", false, WikiTextToken.TokenType.Header2Start, WikiTextTokenScopes.LineStart, x => x | WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("==", false, WikiTextToken.TokenType.Header2End, WikiTextTokenScopes.HeaderText, x => (x | WikiTextTokenScopes.HeaderSuffix) & ~WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("===", false, WikiTextToken.TokenType.Header3Start, WikiTextTokenScopes.LineStart, x => x | WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("===", false, WikiTextToken.TokenType.Header3End, WikiTextTokenScopes.HeaderText, x => (x | WikiTextTokenScopes.HeaderSuffix) & ~WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("====", false, WikiTextToken.TokenType.Header4Start, WikiTextTokenScopes.LineStart, x => x | WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("====", false, WikiTextToken.TokenType.Header4End, WikiTextTokenScopes.HeaderText, x => (x | WikiTextTokenScopes.HeaderSuffix) & ~WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("=====", false, WikiTextToken.TokenType.Header5Start, WikiTextTokenScopes.LineStart, x => x | WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("=====", false, WikiTextToken.TokenType.Header5End, WikiTextTokenScopes.HeaderText, x => (x | WikiTextTokenScopes.HeaderSuffix) & ~WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("======", false, WikiTextToken.TokenType.Header6Start, WikiTextTokenScopes.LineStart, x => x | WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("======", false, WikiTextToken.TokenType.Header6End, WikiTextTokenScopes.HeaderText, x => (x | WikiTextTokenScopes.HeaderSuffix) & ~WikiTextTokenScopes.HeaderText),
                new WikiTextTokenDef("#", false, WikiTextToken.TokenType.HeaderAnchor, WikiTextTokenScopes.HeaderSuffix),
                new WikiTextTokenDef(@"[\*]", true, WikiTextToken.TokenType.BulletList, WikiTextTokenScopes.LineStart, x => WikiTextTokenScopes.InnerText),
                new WikiTextTokenDef(@"[#]", true, WikiTextToken.TokenType.NumberedList, WikiTextTokenScopes.LineStart, x => WikiTextTokenScopes.InnerText),
                new WikiTextTokenDef(" ", false, WikiTextToken.TokenType.ExternalLinkUrlLeadingSpace, WikiTextTokenScopes.ExternalLinkUrl),
                new WikiTextTokenDef(@"[\S^\]]", true, WikiTextToken.TokenType.ExternalLinkUrl, WikiTextTokenScopes.ExternalLinkUrl, x => WikiTextTokenScopes.ExternalLinkText)
            };

            def.Sort ((a, b) => -a.TokenStringLength.CompareTo(b.TokenStringLength));
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