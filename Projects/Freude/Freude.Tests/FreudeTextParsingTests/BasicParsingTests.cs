using System;
using Freude.DocModel;
using Freude.Parsing;
using NUnit.Framework;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class BasicParsingTests
    {
        [Test]
        public void SingleLineTextParagraph()
        {
            doc = parser.ParseText (@" this is some text ");
            Assert.AreEqual (1, doc.Children.Count);
            var par = AssertElement<ParagraphElement>(doc, 0);
            AssertElement<TextElement> (par, 0, x => Assert.AreEqual ("this is some text", x.Text));
        }

        [Test]
        public void MultilineTextParagraphIsMergedIntoSingleLineText()
        {
            doc = parser.ParseText (@"this is some text
   and this too
and this too ");
            Assert.AreEqual (1, doc.Children.Count);
            var par = AssertElement<ParagraphElement>(doc, 0);
            AssertElement<TextElement> (par, 0, x => Assert.AreEqual (@"this is some text and this too and this too", x.Text));
        }

        [Test]
        public void ParagraphsAreSplitByEmptyLines()
        {
            doc = parser.ParseText (@" par 1 line 1
   par 1 line 2

par 2 line 1
par 2 line 2 ");
            Assert.AreEqual (2, doc.Children.Count);
            var par = AssertElement<ParagraphElement> (doc, 0);
            AssertElement<TextElement> (par, 0, x => Assert.AreEqual (@"par 1 line 1 par 1 line 2", x.Text));
            par = AssertElement<ParagraphElement> (doc, 1);
            AssertElement<TextElement> (par, 0, x => Assert.AreEqual (@"par 2 line 1 par 2 line 2", x.Text));
        }

        [Test]
        public void MultipleEmptyLinesBetweenParagraphsAreTreatedAsSingleOne()
        {
            doc = parser.ParseText (@" par 1 line 1
   par 1 line 2

   

par 2 line 1
par 2 line 2 ");
            Assert.AreEqual (2, doc.Children.Count);
            var par = AssertElement<ParagraphElement> (doc, 0);
            AssertElement<TextElement> (par, 0, x => Assert.AreEqual (@"par 1 line 1 par 1 line 2", x.Text));
            par = AssertElement<ParagraphElement> (doc, 1);
            AssertElement<TextElement> (par, 0, x => Assert.AreEqual (@"par 2 line 1 par 2 line 2", x.Text));
        }

        [Test]
        public void EmptyLineCanHaveWhitespaceThatIsIgnored()
        {
            doc = parser.ParseText (@" par 1 line 1
   par 1 line 2
     
par 2 line 1
par 2 line 2 ");
            Assert.AreEqual (2, doc.Children.Count);
            var par = AssertElement<ParagraphElement> (doc, 0);
            AssertElement<TextElement> (par, 0, x => Assert.AreEqual (@"par 1 line 1 par 1 line 2", x.Text));
            par = AssertElement<ParagraphElement> (doc, 1);
            AssertElement<TextElement> (par, 0, x => Assert.AreEqual (@"par 2 line 1 par 2 line 2", x.Text));
        }

        [Test]
        public void SingleImageDoc()
        {
            doc = parser.ParseText (@"[[http://www.arso.gov.si/vreme/napovedi%20in%20podatki/radar_anim.gif]]");
            var par = AssertElement<ParagraphElement> (doc, 0);
            Assert.AreEqual (1, par.Children.Count);
            AssertElement<ImageElement>(par, 0, x => Assert.AreEqual("http://www.arso.gov.si/vreme/napovedi%20in%20podatki/radar_anim.gif", x.ImageUrl));
        }

        [Test] 
        public void DoubleImagesDoc()
        {
            doc = parser.ParseText (@"[[http://www.arso.gov.si/vreme/napovedi%20in%20podatki/radar_anim.gif]] [[http://google.com/test.png]]");
            var par = AssertElement<ParagraphElement> (doc, 0);
            Assert.AreEqual (2, par.Children.Count);
            AssertElement<ImageElement>(par, 0, x => Assert.AreEqual ("http://www.arso.gov.si/vreme/napovedi%20in%20podatki/radar_anim.gif", x.ImageUrl));
            AssertElement<ImageElement>(par, 1, x => Assert.AreEqual ("http://google.com/test.png", x.ImageUrl));
        }

        [Test] 
        public void ImageWithTextAround()
        {
            doc = parser.ParseText (@"text before [[http://www.arso.gov.si/vreme/napovedi%20in%20podatki/radar_anim.gif]] text after");
            var par = AssertElement<ParagraphElement> (doc, 0);
            Assert.AreEqual (3, par.Children.Count);
            AssertElement<TextElement>(par, 0, x => Assert.AreEqual ("text before", x.Text));
            AssertElement<ImageElement>(par, 1, x => Assert.AreEqual ("http://www.arso.gov.si/vreme/napovedi%20in%20podatki/radar_anim.gif", x.ImageUrl));
            AssertElement<TextElement>(par, 2, x => Assert.AreEqual ("text after", x.Text));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void StartingHashIsHeaderPrefix(int headerLevel)
        {
            doc = parser.ParseText (new string('#', headerLevel) + " header");
            Assert.AreEqual (1, doc.Children.Count);
            AssertElement<HeaderElement> (doc, 0, x =>
            {
                Assert.AreEqual("header", x.HeaderText);
                Assert.AreEqual (headerLevel, x.HeaderLevel);
            });            
        }

        [Test]
        public void WhitespaceBeforeHashIsNotHeaderPrefix()
        {
            doc = parser.ParseText (@"  # header");
            var par = AssertElement<ParagraphElement> (doc, 0);
            Assert.AreEqual (1, par.Children.Count);
            AssertElement<TextElement> (par, 0, x => Assert.AreEqual ("# header", x.Text));            
        }

        [SetUp]
        public void Setup()
        {
            parser = new FreudeTextParser ();
        }

        private static TElement AssertElement<TElement>(IDocumentElementContainer container, int index, Action<TElement> assertAction = null)
            where TElement : IDocumentElement
        {
            IDocumentElement element = container.Children[index];
            Assert.IsInstanceOf<TElement> (element);
            if (assertAction != null)
                assertAction((TElement)element);

            return (TElement)element;
        }

        private FreudeTextParser parser;
        private DocumentDef doc;
    }
}