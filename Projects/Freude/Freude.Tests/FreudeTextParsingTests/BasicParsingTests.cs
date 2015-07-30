using NUnit.Framework;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class BasicParsingTests
    {
        [Test]
        public void SingleImageDoc()
        {
            FreudeTextParser parser = new FreudeTextParser();
            DocumentDef doc = parser.ParseText (@"[[http://www.arso.gov.si/vreme/napovedi%20in%20podatki/radar_anim.gif]]");
            Assert.AreEqual(1, doc.Children.Count);
            Assert.IsInstanceOf<ImageElement>(doc.Children[0]);
            Assert.AreEqual("http://www.arso.gov.si/vreme/napovedi%20in%20podatki/radar_anim.gif", ((ImageElement)doc.Children[0]).ImageUrl);
        }

        [Test] 
        public void DoubleImagesDoc()
        {
            FreudeTextParser parser = new FreudeTextParser();
            DocumentDef doc = parser.ParseText (@"[[http://www.arso.gov.si/vreme/napovedi%20in%20podatki/radar_anim.gif]] [[http://google.com/test.png]]");
            Assert.AreEqual(2, doc.Children.Count);
            Assert.IsInstanceOf<ImageElement>(doc.Children[0]);
            Assert.AreEqual("http://www.arso.gov.si/vreme/napovedi%20in%20podatki/radar_anim.gif", ((ImageElement)doc.Children[0]).ImageUrl);
            Assert.IsInstanceOf<ImageElement>(doc.Children[1]);
            Assert.AreEqual ("http://google.com/test.png", ((ImageElement)doc.Children[1]).ImageUrl);
        } 

        [Test] 
        public void ImageWithTextAround()
        {
            FreudeTextParser parser = new FreudeTextParser();
            DocumentDef doc = parser.ParseText (@"text before [[http://www.arso.gov.si/vreme/napovedi%20in%20podatki/radar_anim.gif]] text after");
            Assert.AreEqual(3, doc.Children.Count);
            Assert.IsInstanceOf<TextElement> (doc.Children[0]);
            Assert.AreEqual ("text before", ((TextElement)doc.Children[0]).Text);
            Assert.IsInstanceOf<ImageElement> (doc.Children[1]);
            Assert.AreEqual("http://www.arso.gov.si/vreme/napovedi%20in%20podatki/radar_anim.gif", ((ImageElement)doc.Children[1]).ImageUrl);
            Assert.IsInstanceOf<TextElement> (doc.Children[2]);
            Assert.AreEqual ("text after", ((TextElement)doc.Children[2]).Text);
        } 
    }
}