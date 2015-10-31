using System.Collections.Generic;
using Freude.Parsing;
using NUnit.Framework;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class TokenizerTests
    {
        [Test]
        public void TokenizeStuff()
        {
            IList<WikiTextToken> tokens = fixture.TokenizePart(" text [[link  ]] text 2");
            Assert.AreEqual(5, tokens.Count);
            Assert.AreEqual(WikiTextToken.TokenType.Text, tokens[0].Type);
            Assert.AreEqual(" text ", tokens[0].Text);
            Assert.AreEqual (WikiTextToken.TokenType.DoubleSquareBracketsOpen, tokens[1].Type);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[2].Type);
            Assert.AreEqual ("link  ", tokens[2].Text);
            Assert.AreEqual (WikiTextToken.TokenType.DoubleSquareBracketsClose, tokens[3].Type);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[4].Type);
            Assert.AreEqual (" text 2", tokens[4].Text);
        }

        [Test]
        public void EndingToken()
        {
            IList<WikiTextToken> tokens = fixture.TokenizePart("text]");
            Assert.AreEqual (2, tokens.Count);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[0].Type);
            Assert.AreEqual ("text", tokens[0].Text);
            Assert.AreEqual (WikiTextToken.TokenType.SingleSquareBracketsClose, tokens[1].Type);
        }

        [Test]
        public void LineBeginningOnlyTokens()
        {
            IList<WikiTextToken> tokens = fixture.TokenizeWholeLine("==text===");
            Assert.AreEqual (3, tokens.Count);
            Assert.AreEqual (WikiTextToken.TokenType.Header2Start, tokens[0].Type);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[1].Type);
            Assert.AreEqual ("text", tokens[1].Text);
            Assert.AreEqual (WikiTextToken.TokenType.Header3End, tokens[2].Type);
        }

        [SetUp]
        public void Setup ()
        {
            fixture = new ParsingFixture ();
        }

        private ParsingFixture fixture;
    }
}
