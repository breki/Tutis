using Freude.DocModel;
using NUnit.Framework;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class InternalLinkParsingTests
    {
        [Test]
        public void SimpleLink()
        {
            fixture.Parse ("[[Main Page]]")
                .AssertNoErrrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual(1, par.ChildrenCount);
            var link = fixture.AssertElement<InternalLinkElement> (par, 0);
            Assert.AreEqual ("Main Page", link.LinkName);
        }

        [Test]
        public void PageNameShouldBeTrimmed()
        {
            fixture.Parse ("[[ Main Page ]]")
                .AssertNoErrrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual(1, par.ChildrenCount);
            var link = fixture.AssertElement<InternalLinkElement> (par, 0);
            Assert.AreEqual ("Main Page", link.LinkName);
        }

        [Test]
        public void TextLinkMix()
        {
            fixture.Parse ("before [[ Main Page ]] after")
                .AssertNoErrrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (3, par.ChildrenCount);
            fixture.AssertText(par, 0, "before ");
            var link = fixture.AssertElement<InternalLinkElement> (par, 1);
            Assert.AreEqual ("Main Page", link.LinkName);
            fixture.AssertText(par, 2, " after");
        }

        [Test]
        public void PageNameShouldNotBeEmpty()
        {
            fixture.Parse("[[ ]]").AssertError("Internal link has an empty page name");
        }

        [Test]
        public void ClosingBracketsAreMissing()
        {
            fixture.Parse ("[[ Main Page").AssertError ("Missing token ']]'");
        }

        [Test]
        public void PipedLink()
        {
            fixture.Parse ("[[ Main Page | real name ]]")
                .AssertNoErrrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual(1, par.ChildrenCount);
            var link = fixture.AssertElement<InternalLinkElement> (par, 0);
            Assert.AreEqual ("Main Page", link.LinkName);
            Assert.AreEqual ("real name", link.LinkDescription);
        }

        [Test]
        public void PipedLinkWithEmptyDescription()
        {
            fixture.Parse ("[[ Main Page |  ]]")
                .AssertError ("Internal link ('Main Page') has an empty description");
        }

        [SetUp]
        public void Setup ()
        {
            fixture = new ParsingFixture ();
        }

        private ParsingFixture fixture;
    }
}