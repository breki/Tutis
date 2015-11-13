using System;
using Freude.DocModel;
using Freude.HtmlGenerating;
using NUnit.Framework;

namespace Freude.Tests.HtmlGeneratingTests
{
    public class BasicGeneratingTests
    {
        [Test]
        public void SimpleParagraph()
        {
            ParagraphElement par = new ParagraphElement(ParagraphElement.ParagraphType.Regular, 0);
            par.Children.Add(new TextElement("this is text"));
            doc.Children.Add(par);

            string html = generator.GenerateHtml(doc);
            Assert.AreEqual("<p>this is text</p>", html);
        }

        [Test]
        public void EscapeText()
        {
            ParagraphElement par = new ParagraphElement(ParagraphElement.ParagraphType.Regular, 0);
            par.Children.Add(new TextElement("this is text < & >"));
            doc.Children.Add(par);

            string html = generator.GenerateHtml(doc);
            Assert.AreEqual ("<p>this is text &lt; &amp; &gt;</p>", html);
        }

        [Test]
        public void ParagraphWithImage()
        {
            ParagraphElement par = new ParagraphElement(ParagraphElement.ParagraphType.Regular, 0);
            par.Children.Add(new TextElement("text before"));
            par.Children.Add (new ImageElement (new Uri("http://www.arso.gov.si/vreme/napovedi%20in%20podatki/radar_anim.gif")));
            par.Children.Add(new TextElement("text after"));
            doc.Children.Add(par);

            string html = generator.GenerateHtml(doc);
            Assert.AreEqual ("<p>text before<img src=\"http://www.arso.gov.si/vreme/napovedi in podatki/radar_anim.gif\" />text after</p>", html);
        }

        [Test]
        public void HeadingParagraph ()
        {
            HeadingElement el = new HeadingElement("heading", 3);
            doc.Children.Add (el);

            string html = generator.GenerateHtml (doc);
            Assert.AreEqual ("<h3>heading</h3>", html);
        }

        [SetUp]
        public void Setup()
        {
            generator = new HtmlGenerator();
            doc = new DocumentDef ();
        }

        private HtmlGenerator generator;
        private DocumentDef doc;
    }
}