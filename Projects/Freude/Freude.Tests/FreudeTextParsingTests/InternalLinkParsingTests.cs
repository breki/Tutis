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
        public void PageNameShouldNotBeEmpty()
        {
            fixture.Parse("[[ ]]").AssertErrror("Internal link has an empty page name");
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

        [SetUp]
        public void Setup ()
        {
            fixture = new ParsingFixture ();
        }

        private ParsingFixture fixture;
    }
}