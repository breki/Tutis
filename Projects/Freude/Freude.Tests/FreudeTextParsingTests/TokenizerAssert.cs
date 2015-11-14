using System.Collections.Generic;
using Freude.Parsing;
using NUnit.Framework;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class TokenizerAssert
    {
        public TokenizerAssert(IList<WikiTextToken> tokens)
        {
            this.tokens = tokens;
        }

        public TokenizerAssert TokensCount(int expecteCount)
        {
            Assert.AreEqual(expecteCount, tokens.Count);
            return this;
        }

        public TokenizerAssert Expect (WikiTextToken.TokenType tokenType)
        {
            Assert.AreEqual (tokenType, tokens[cursor].Type);
            cursor++;
            return this;
        }

        public TokenizerAssert Expect (WikiTextToken.TokenType tokenType, string expectedText)
        {
            Assert.AreEqual (tokenType, tokens[cursor].Type);
            Assert.AreEqual (expectedText, tokens[cursor].Text);
            cursor++;
            return this;
        }

        public TokenizerAssert ExpectText(string expectedText)
        {
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[cursor].Type);
            Assert.AreEqual (expectedText, tokens[cursor].Text);
            cursor++;
            return this;
        }

        private readonly IList<WikiTextToken> tokens;
        private int cursor;
    }
}