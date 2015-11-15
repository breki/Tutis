using System;
using Freude.DocModel;
using NUnit.Framework;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class ExternalLinkParsingTests
    {
        [Test]
        public void SimpleCase()
        {
            fixture.Parse ("[http://google.com]")
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (1, par.ChildrenCount);
            var link = fixture.AssertElement<ExternalLinkElement> (par, 0);
            Assert.AreEqual (new Uri("http://google.com"), link.Url);
        }
        
        [Test]
        public void WithWhiteSpaceAround()
        {
            fixture.Parse ("[ http://google.com  ]")
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (1, par.ChildrenCount);
            var link = fixture.AssertElement<ExternalLinkElement> (par, 0);
            Assert.AreEqual (new Uri("http://google.com"), link.Url);
        }
        
        [Test]
        public void WithDescription()
        {
            fixture.Parse ("[ http://google.com   Google Website ]")
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (1, par.ChildrenCount);
            var link = fixture.AssertElement<ExternalLinkElement> (par, 0);
            Assert.AreEqual (new Uri("http://google.com"), link.Url);
            Assert.AreEqual ("Google Website", link.LinkDescription);
        }

        [Test]
        public void ClosingBracketIsMissing ()
        {
            fixture.Parse ("[http://google.com").AssertError ("Missing token ']'");
        }

        [Test]
        public void ClosingBracketIsMissing2 ()
        {
            fixture.Parse ("[ http://google.com  Google Website ").AssertError ("Missing token ']'");
        }

        [Test]
        public void InvalidUrl()
        {
            fixture.Parse ("[google.com]")
                .AssertError ("Invalid external link URL 'google.com': Invalid URI: The format of the URI could not be determined.");
        }

        [Test]
        public void IfSpaceIsBetweenTextAndLinkItShouldBePreserved()
        {
            const string Text = @"First line.
See [http://google.com] section for more details.";

            fixture.Parse (Text)
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (3, par.ChildrenCount);
            fixture.AssertText (par, 0, "First line. See ");
            var link = fixture.AssertElement<ExternalLinkElement> (par, 1);
            fixture.AssertText (par, 2, " section for more details.");
        }

        [SetUp]
        public void Setup ()
        {
            fixture = new ParsingFixture ();
        }

        private ParsingFixture fixture;        
    }
}