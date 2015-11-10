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

        [Test]
        public void HeaderAnchor()
        {
            IList<WikiTextToken> tokens = fixture.TokenizeWholeLine ("==text==#anchor");
            Assert.AreEqual (5, tokens.Count);
            Assert.AreEqual (WikiTextToken.TokenType.Header2Start, tokens[0].Type);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[1].Type);
            Assert.AreEqual ("text", tokens[1].Text);
            Assert.AreEqual (WikiTextToken.TokenType.Header2End, tokens[2].Type);
            Assert.AreEqual (WikiTextToken.TokenType.HeaderAnchor, tokens[3].Type);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[4].Type);
            Assert.AreEqual ("anchor", tokens[4].Text);
        }

        [Test]
        public void BoldInMiddleTest()
        {
            IList<WikiTextToken> tokens = fixture.TokenizeWholeLine ("There is something '''bold''' in here");
            Assert.AreEqual (5, tokens.Count);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[0].Type);
            Assert.AreEqual (WikiTextToken.TokenType.TripleApostrophe, tokens[1].Type);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[2].Type);
            Assert.AreEqual (WikiTextToken.TokenType.TripleApostrophe, tokens[3].Type);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[4].Type);
        }

        [Test]
        public void BoldAtStartTest()
        {
            IList<WikiTextToken> tokens = fixture.TokenizeWholeLine ("'''bold''' in here");
            Assert.AreEqual (4, tokens.Count);
            Assert.AreEqual (WikiTextToken.TokenType.TripleApostrophe, tokens[0].Type);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[1].Type);
            Assert.AreEqual (WikiTextToken.TokenType.TripleApostrophe, tokens[2].Type);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[3].Type);
        }

        [Test]
        public void ItalicTest()
        {
            IList<WikiTextToken> tokens = fixture.TokenizeWholeLine ("There is something ''italic'' in here");
            Assert.AreEqual (5, tokens.Count);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[0].Type);
            Assert.AreEqual (WikiTextToken.TokenType.DoubleApostrophe, tokens[1].Type);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[2].Type);
            Assert.AreEqual (WikiTextToken.TokenType.DoubleApostrophe, tokens[3].Type);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[4].Type);
        }

        [Test]
        public void TokenizeInternalLink()
        {
            IList<WikiTextToken> tokens = fixture.TokenizeWholeLine(" text [[link  ]] text 2");
            Assert.AreEqual (5, tokens.Count);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[0].Type);
            Assert.AreEqual (" text ", tokens[0].Text);
            Assert.AreEqual (WikiTextToken.TokenType.DoubleSquareBracketsOpen, tokens[1].Type);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[2].Type);
            Assert.AreEqual ("link  ", tokens[2].Text);
            Assert.AreEqual (WikiTextToken.TokenType.DoubleSquareBracketsClose, tokens[3].Type);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[4].Type);
            Assert.AreEqual (" text 2", tokens[4].Text);
        }

        [Test]
        public void TokenizeInternalLinkWithNamespaces()
        {
            IList<WikiTextToken> tokens = fixture.TokenizeWholeLine("[[ns : link  ]]");
            Assert.AreEqual (5, tokens.Count);
            Assert.AreEqual (WikiTextToken.TokenType.DoubleSquareBracketsOpen, tokens[0].Type);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[1].Type);
            Assert.AreEqual ("ns ", tokens[1].Text);
            Assert.AreEqual (WikiTextToken.TokenType.NamespaceSeparator, tokens[2].Type);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[3].Type);
            Assert.AreEqual (" link  ", tokens[3].Text);
            Assert.AreEqual (WikiTextToken.TokenType.DoubleSquareBracketsClose, tokens[4].Type);
        }

        [Test]
        public void TokenizeExternalLink()
        {
            IList<WikiTextToken> tokens = fixture.TokenizeWholeLine (" text [  http://google.com  Google Website ] text 2");
            Assert.AreEqual (8, tokens.Count);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[0].Type);
            Assert.AreEqual (" text ", tokens[0].Text);
            Assert.AreEqual (WikiTextToken.TokenType.SingleSquareBracketsOpen, tokens[1].Type);
            Assert.AreEqual (WikiTextToken.TokenType.ExternalLinkUrlLeadingSpace, tokens[2].Type);
            Assert.AreEqual (WikiTextToken.TokenType.ExternalLinkUrlLeadingSpace, tokens[3].Type);
            Assert.AreEqual (WikiTextToken.TokenType.ExternalLinkUrl, tokens[4].Type);
            Assert.AreEqual ("http://google.com", tokens[4].Text);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[5].Type);
            Assert.AreEqual ("  Google Website ", tokens[5].Text);
            Assert.AreEqual (WikiTextToken.TokenType.SingleSquareBracketsClose, tokens[6].Type);
            Assert.AreEqual (WikiTextToken.TokenType.Text, tokens[7].Type);
            Assert.AreEqual (" text 2", tokens[7].Text);
        }

        [Test]
        public void TokenizeExternalLinkSimple()
        {
            IList<WikiTextToken> tokens = fixture.TokenizeWholeLine ("[http://google.com]");
            Assert.AreEqual (3, tokens.Count);
            Assert.AreEqual (WikiTextToken.TokenType.SingleSquareBracketsOpen, tokens[0].Type);
            Assert.AreEqual (WikiTextToken.TokenType.ExternalLinkUrl, tokens[1].Type);
            Assert.AreEqual ("http://google.com", tokens[1].Text);
            Assert.AreEqual (WikiTextToken.TokenType.SingleSquareBracketsClose, tokens[2].Type);
        }

        [SetUp]
        public void Setup ()
        {
            fixture = new ParsingFixture ();
        }

        private ParsingFixture fixture;
    }
}
