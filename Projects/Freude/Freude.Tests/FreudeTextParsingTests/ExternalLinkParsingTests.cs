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
        public void WithWhitespaceAround()
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
        
        [SetUp]
        public void Setup ()
        {
            fixture = new ParsingFixture ();
        }

        private ParsingFixture fixture;        
    }
}