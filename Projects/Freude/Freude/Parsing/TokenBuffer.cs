using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Freude.Parsing
{
    internal class TokenBuffer
    {
        public TokenBuffer(IList<WikiTextToken> tokens)
        {
            Contract.Requires(tokens != null);
            this.tokens = tokens;
        }

        public bool EndOfTokens
        {
            get { return tokenIndex >= tokens.Count; }
        }

        public WikiTextToken Token
        {
            get
            {
                Contract.Ensures(Contract.Result<WikiTextToken>() != null);

                if (EndOfTokens)
                    throw new InvalidOperationException("No more tokens");

                return tokens[tokenIndex];
            }
        }

        public void MoveToNextToken()
        {
            if (EndOfTokens)
                throw new InvalidOperationException("No more tokens");

            tokenIndex++;
        }

        public bool ProcessUntilEnd (Func<WikiTextToken, bool> tokenFunc)
        {
            Contract.Requires (tokenFunc != null);

            while (!EndOfTokens)
            {
                WikiTextToken token = Token;
                if (!tokenFunc (token))
                    return false;

                MoveToNextToken ();
            }

            return true;
        }

        public WikiTextToken.TokenType? ProcessUntilToken (
            ParsingContext context,
            Func<WikiTextToken, bool> tokenFunc,
            params WikiTextToken.TokenType[] untilTypes)
        {
            Contract.Requires (context != null);
            Contract.Requires (tokenFunc != null);

            while (!EndOfTokens)
            {
                WikiTextToken token = Token;
                if (untilTypes.Contains (token.Type))
                    return token.Type;

                if (!tokenFunc (token))
                    return null;

                MoveToNextToken ();
            }

            context.ReportError ("Expected one of token types ({0}), but they are missing".Fmt (untilTypes.Concat (x => x.ToString (), ",")));
            return null;
        }

        public WikiTextToken ExpectToken (ParsingContext context, WikiTextToken.TokenType expectedTokenType)
        {
            Contract.Requires (context != null);

            if (EndOfTokens)
            {
                context.ReportError ("Unexpected end, expected token '{0}'".Fmt (expectedTokenType));
                return null;
            }

            WikiTextToken token = Token;
            if (token.Type != WikiTextToken.TokenType.Text)
            {
                context.ReportError ("Expected token '{0}' but got '{1}".Fmt (expectedTokenType, token.Type));
                return null;
            }

            return token;
        }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(tokens != null);
            Contract.Invariant(Contract.ForAll(tokens, x => x != null));
            Contract.Invariant(tokenIndex >= 0 && tokenIndex <= tokens.Count);
        }

        private readonly IList<WikiTextToken> tokens;
        private int tokenIndex;
    }
}