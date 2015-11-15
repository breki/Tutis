using Freude.Parsing;
using NUnit.Framework;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class TokenizerTests
    {
        [Test]
        public void TokenizeStuff()
        {
            fixture.TokenizePart(" text [[link  ]] text 2")
                .TokensCount(5)
                .ExpectText(" text ")
                .Expect(WikiTextToken.TokenType.DoubleSquareBracketsOpen)
                .ExpectText("link  ")
                .Expect(WikiTextToken.TokenType.DoubleSquareBracketsClose)
                .ExpectText(" text 2");
        }

        [Test]
        public void EndingToken()
        {
            fixture.TokenizePart("text]")
                .TokensCount(2)
                .ExpectText("text")
                .Expect(WikiTextToken.TokenType.SingleSquareBracketsClose);
        }

        [Test]
        public void LineBeginningOnlyTokens()
        {
            fixture.TokenizeWholeLine("==text===")
                .TokensCount(3)
                .Expect(WikiTextToken.TokenType.Heading2Start)
                .ExpectText("text")
                .Expect(WikiTextToken.TokenType.Heading3End);
        }

        [Test]
        public void HeadingAnchor()
        {
            fixture.TokenizeWholeLine ("==text==#anchor")
                .TokensCount(5)
                .Expect(WikiTextToken.TokenType.Heading2Start)
                .ExpectText("text")
                .Expect(WikiTextToken.TokenType.Heading2End)
                .Expect(WikiTextToken.TokenType.HeadingAnchor)
                .ExpectText("anchor");
        }

        [Test]
        public void BoldInMiddleTest ()
        {
            fixture.TokenizeWholeLine("There is something '''bold''' in here")
                .TokensCount(5)
                .Expect(WikiTextToken.TokenType.Text)
                .Expect(WikiTextToken.TokenType.TripleApostrophe)
                .Expect(WikiTextToken.TokenType.Text)
                .Expect(WikiTextToken.TokenType.TripleApostrophe)
                .Expect(WikiTextToken.TokenType.Text);
        }

        [Test]
        public void BoldAtStartTest ()
        {
            fixture.TokenizeWholeLine("'''bold''' in here")
                .TokensCount(4)
                .Expect(WikiTextToken.TokenType.TripleApostrophe)
                .Expect(WikiTextToken.TokenType.Text)
                .Expect(WikiTextToken.TokenType.TripleApostrophe)
                .Expect(WikiTextToken.TokenType.Text);
        }

        [Test]
        public void ItalicTest ()
        {
            fixture.TokenizeWholeLine("There is something ''italic'' in here")
                .TokensCount(5)
                .Expect(WikiTextToken.TokenType.Text)
                .Expect(WikiTextToken.TokenType.DoubleApostrophe)
                .Expect(WikiTextToken.TokenType.Text)
                .Expect(WikiTextToken.TokenType.DoubleApostrophe)
                .Expect(WikiTextToken.TokenType.Text);
        }

        [Test]
        public void TokenizeInternalLink ()
        {
            fixture.TokenizeWholeLine(" text [[link  ]] text 2")
                .TokensCount(5)
                .ExpectText(" text ")
                .Expect(WikiTextToken.TokenType.DoubleSquareBracketsOpen)
                .ExpectText("link  ")
                .Expect(WikiTextToken.TokenType.DoubleSquareBracketsClose)
                .ExpectText(" text 2");
        }

        [Test]
        public void TokenizeInternalLinkWithNamespaces ()
        {
            fixture.TokenizeWholeLine ("[[ns : link  ]]")
                .TokensCount (5)
                .Expect (WikiTextToken.TokenType.DoubleSquareBracketsOpen)
                .ExpectText ("ns ")
                .Expect (WikiTextToken.TokenType.NamespaceSeparator)
                .ExpectText (" link  ")
                .Expect (WikiTextToken.TokenType.DoubleSquareBracketsClose);
        }

        [Test, Ignore("todo")]
        public void TokenizeInternalLinkWithSeveralPipes()
        {
            fixture.TokenizeWholeLine ("[[image:monkey.png||]]")
                .TokensCount (5)
                .Expect (WikiTextToken.TokenType.DoubleSquareBracketsOpen)
                .ExpectText ("ns ")
                .Expect (WikiTextToken.TokenType.NamespaceSeparator)
                .ExpectText (" link  ")
                .Expect (WikiTextToken.TokenType.DoubleSquareBracketsClose);
        }

        [Test]
        public void TokenizeExternalLink ()
        {
            fixture.TokenizeWholeLine (" text [  http://google.com  Google Website ] text 2")
                .TokensCount(8)
                .ExpectText(" text ")
                .Expect(WikiTextToken.TokenType.SingleSquareBracketsOpen)
                .Expect(WikiTextToken.TokenType.ExternalLinkUrlLeadingSpace)
                .Expect(WikiTextToken.TokenType.ExternalLinkUrlLeadingSpace)
                .Expect(WikiTextToken.TokenType.ExternalLinkUrl, "http://google.com")
                .ExpectText("  Google Website ")
                .Expect(WikiTextToken.TokenType.SingleSquareBracketsClose)
                .ExpectText(" text 2");
        }

        [Test]
        public void TokenizeExternalLinkSimple ()
        {
            fixture.TokenizeWholeLine ("[http://google.com]")
                .TokensCount(3)
                .Expect(WikiTextToken.TokenType.SingleSquareBracketsOpen)
                .Expect(WikiTextToken.TokenType.ExternalLinkUrl, "http://google.com")
                .Expect(WikiTextToken.TokenType.SingleSquareBracketsClose);
        }

        [Test]
        public void TokenizeBulletList ()
        {
            fixture.TokenizeWholeLine ("* something here")
                .TokensCount(2)
                .Expect(WikiTextToken.TokenType.BulletList)
                .ExpectText(" something here");
        }

        [Test]
        public void TokenizeBulletListWithExtraBullets ()
        {
            fixture.TokenizeWholeLine("* something *here*")
                .TokensCount(2)
                .Expect(WikiTextToken.TokenType.BulletList)
                .ExpectText(" something *here*");
        }

        [Test]
        public void TokenizeBulletListWithStyling ()
        {
            fixture.TokenizeWholeLine("* something '''here'''")
                .TokensCount(5)
                .Expect(WikiTextToken.TokenType.BulletList)
                .ExpectText(" something ")
                .Expect(WikiTextToken.TokenType.TripleApostrophe)
                .ExpectText("here")
                .Expect(WikiTextToken.TokenType.TripleApostrophe);
        }

        [Test]
        public void TokenizeIndentedBulletList ()
        {
            fixture.TokenizeWholeLine("** something here")
                .TokensCount(2)
                .Expect(WikiTextToken.TokenType.BulletList)
                .ExpectText(" something here");
        }

        [Test]
        public void TokenizeNumberedListWithExtraHashes ()
        {
            fixture.TokenizeWholeLine("# something #here#")
                .TokensCount(2)
                .Expect(WikiTextToken.TokenType.NumberedList)
                .ExpectText(" something #here#");
        }

        [Test]
        public void TokenizeNumberedListWithStyling ()
        {
            fixture.TokenizeWholeLine("# something '''here'''")
                .TokensCount(5)
                .Expect(WikiTextToken.TokenType.NumberedList)
                .ExpectText(" something ")
                .Expect(WikiTextToken.TokenType.TripleApostrophe)
                .ExpectText("here")
                .Expect(WikiTextToken.TokenType.TripleApostrophe);
        }

        [Test]
        public void TokenizeIndentedNumberedList ()
        {
            fixture.TokenizeWholeLine("## something here")
                .TokensCount(2)
                .Expect(WikiTextToken.TokenType.NumberedList)
                .ExpectText(" something here");
        }

        [Test]
        public void TokenizeIndentedTextWithExtraColons ()
        {
            fixture.TokenizeWholeLine(": something :here:")
                .TokensCount(2)
                .Expect(WikiTextToken.TokenType.Indent)
                .ExpectText(" something :here:");
        }

        [Test]
        public void TokenizeIndentedTextWithStyling ()
        {
            fixture.TokenizeWholeLine(": something '''here'''")
                .TokensCount(5)
                .Expect(WikiTextToken.TokenType.Indent)
                .ExpectText(" something ")
                .Expect(WikiTextToken.TokenType.TripleApostrophe)
                .ExpectText("here")
                .Expect(WikiTextToken.TokenType.TripleApostrophe);
        }

        [Test]
        public void TokenizeMultipleIndentedTextList ()
        {
            fixture.TokenizeWholeLine(":: something here")
                .TokensCount(2)
                .Expect(WikiTextToken.TokenType.Indent)
                .ExpectText(" something here");
        }

        [SetUp]
        public void Setup ()
        {
            fixture = new ParsingFixture ();
        }

        private ParsingFixture fixture;
    }
}
