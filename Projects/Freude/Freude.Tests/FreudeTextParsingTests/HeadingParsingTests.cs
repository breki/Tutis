using Freude.DocModel;
using NUnit.Framework;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class HeadingParsingTests
    {
        [TestCase (1)]
        [TestCase (2)]
        [TestCase (3)]
        public void StartingEqualsIsHeadinhPrefix (int headingLevel)
        {
            string equalsToken = new string ('=', headingLevel);
            fixture.Parse (equalsToken + " heading " + equalsToken)
                .AssertNoErrors ()
                .AssertChildCount (1);

            fixture.AssertElement<HeadingElement> (
                0,
                x =>
                {
                    Assert.AreEqual ("heading", x.HeadingText);
                    Assert.AreEqual (headingLevel, x.HeadingLevel);
                });
        }

        [Test]
        public void HeadingWithAnchor ()
        {
            fixture.Parse (@"== heading==#anchor+test")
                .AssertNoErrors ()
                .AssertChildCount (1);

            fixture.AssertElement<HeadingElement> (
                0,
                x =>
                {
                    Assert.AreEqual ("heading", x.HeadingText);
                    Assert.AreEqual (2, x.HeadingLevel);
                    Assert.AreEqual ("anchor+test", x.AnchorId);
                });
        }

        [Test]
        public void HeadingBetweenParagraphs ()
        {
            fixture.Parse (@"par 1 line 1
par 1 line 2
==heading==
par 2 line 1
par 2 line 2")
                .AssertNoErrors ()
                .AssertChildCount (3);

            var par = fixture.AssertElement<ParagraphElement> (0);
            fixture.AssertElement<TextElement> (par, 0, x => Assert.AreEqual (@"par 1 line 1 par 1 line 2", x.Text));
            fixture.AssertElement<HeadingElement> (1, x => Assert.AreEqual (@"heading", x.HeadingText));
            par = fixture.AssertElement<ParagraphElement> (2);
            fixture.AssertElement<TextElement> (par, 0, x => Assert.AreEqual (@"par 2 line 1 par 2 line 2", x.Text));
        }

        [SetUp]
        public void Setup ()
        {
            fixture = new ParsingFixture ();
        }

        private ParsingFixture fixture;
    }
}