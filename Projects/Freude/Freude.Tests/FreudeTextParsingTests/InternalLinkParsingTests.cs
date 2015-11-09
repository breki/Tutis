﻿using Freude.DocModel;
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
            Assert.AreEqual ("Main Page", link.LinkId.PageName);
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
            Assert.AreEqual ("Main Page", link.LinkId.PageName);
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
            Assert.AreEqual ("Main Page", link.LinkId.PageName);
            fixture.AssertText(par, 2, " after");
        }

        [Test]
        public void PageNameShouldNotBeEmpty()
        {
            fixture.Parse("[[ ]]").AssertError("Internal link has an empty page name");
        }

        [TestCase ("[[ Main Page")]
        [TestCase ("[[ Main Page | ")]
        public void ClosingBracketsAreMissing(string text)
        {
            fixture.Parse (text).AssertError ("Missing token ']]'");
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
            Assert.AreEqual ("Main Page", link.LinkId.PageName);
            Assert.AreEqual ("real name", link.LinkDescription);
        }

        [Test]
        public void PipedLinkWithEmptyDescription()
        {
            fixture.Parse ("[[ Main Page |  ]]")
                .AssertError ("Internal link ('Main Page') has an empty description");
        }

        [Test]
        public void LinkWithNamespaces()
        {
            fixture.Parse ("[[ Category: Something else : Main Page ]]")
                .AssertNoErrrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (1, par.ChildrenCount);
            var link = fixture.AssertElement<InternalLinkElement> (par, 0);
            Assert.AreEqual (2, link.LinkId.NamespacePartsCount);            
            Assert.AreEqual ("Category", link.LinkId.NamespaceParts[0]);            
            Assert.AreEqual ("Something else", link.LinkId.NamespaceParts[1]);            
            Assert.AreEqual ("Main Page", link.LinkId.PageName);            
        }

        [SetUp]
        public void Setup ()
        {
            fixture = new ParsingFixture ();
        }

        private ParsingFixture fixture;
    }
}