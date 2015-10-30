using Freude.DocModel;
using NUnit.Framework;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class HeaderParsingTests
    {
        [TestCase (1)]
        [TestCase (2)]
        [TestCase (3)]
        public void StartingHashIsHeaderPrefix (int headerLevel)
        {
            fixture.Parse (new string ('#', headerLevel) + " header")
                .AssertNoErrrors ()
                .AssertChildCount (1);

            fixture.AssertElement<HeaderElement> (
                0,
                x =>
                {
                    Assert.AreEqual ("header", x.HeaderText);
                    Assert.AreEqual (headerLevel, x.HeaderLevel);
                });
        }

        [TestCase (1)]
        [TestCase (2)]
        [TestCase (3)]
        public void StartingEqualsIsHeaderPrefix (int headerLevel)
        {
            string equalsToken = new string ('=', headerLevel);
            fixture.Parse (equalsToken + " header " + equalsToken)
                .AssertNoErrrors ()
                .AssertChildCount (1);

            fixture.AssertElement<HeaderElement> (
                0,
                x =>
                {
                    Assert.AreEqual ("header", x.HeaderText);
                    Assert.AreEqual (headerLevel, x.HeaderLevel);
                });
        }

        [Test]
        public void HeaderWithAnchor ()
        {
            fixture.Parse (@"== header== #anchor+test")
                .AssertNoErrrors ()
                .AssertChildCount (1);

            fixture.AssertElement<HeaderElement> (
                0,
                x =>
                {
                    Assert.AreEqual ("header", x.HeaderText);
                    Assert.AreEqual (2, x.HeaderLevel);
                    Assert.AreEqual ("anchor+test", x.AnchorId);
                });
        }

        [Test]
        public void WhiteSpaceBeforeHashIsNotHeaderPrefix ()
        {
            fixture.Parse (@"  # header")
                .AssertNoErrrors ();

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (1, par.Children.Count);
            fixture.AssertElement<TextElement> (par, 0, x => Assert.AreEqual ("# header", x.Text));
        }

        [Test]
        public void HeaderBetweenParagraphs ()
        {
            fixture.Parse (@" par 1 line 1
   par 1 line 2
#header
par 2 line 1
par 2 line 2 ")
                .AssertNoErrrors ()
                .AssertChildCount (3);

            var par = fixture.AssertElement<ParagraphElement> (0);
            fixture.AssertElement<TextElement> (par, 0, x => Assert.AreEqual (@"par 1 line 1 par 1 line 2", x.Text));
            fixture.AssertElement<HeaderElement> (1, x => Assert.AreEqual (@"header", x.HeaderText));
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