using Freude.DocModel;
using NUnit.Framework;

namespace Freude.Tests.FreudeTextParsingTests
{
    [Ignore("todo next: update the image format")]
    public class ImageParsingTests
    {
        [Test]
        public void SingleImageDoc()
        {
            fixture.Parse (@"[[http://www.arso.gov.si/vreme/napovedi%20in%20podatki/radar_anim.gif]]")
                .AssertNoErrors();

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (1, par.ChildrenCount);
            fixture.AssertElement<ImageElement> (par, 0, x => Assert.AreEqual ("http://www.arso.gov.si/vreme/napovedi in podatki/radar_anim.gif", x.ImageUrl.ToString ()));
        }

        [Test] 
        public void DoubleImagesDoc()
        {
            fixture.Parse(@"[[http://www.arso.gov.si/vreme/napovedi%20in%20podatki/radar_anim.gif]] [[http://google.com/test.png]]")
                .AssertNoErrors();

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (2, par.ChildrenCount);
            fixture.AssertElement<ImageElement> (par, 0, x => Assert.AreEqual ("http://www.arso.gov.si/vreme/napovedi in podatki/radar_anim.gif", x.ImageUrl.ToString ()));
            fixture.AssertElement<ImageElement> (par, 1, x => Assert.AreEqual ("http://google.com/test.png", x.ImageUrl.ToString ()));
        }

        [Test] 
        public void ImageWithTextAround()
        {
            fixture.Parse (@"text before [[http://www.arso.gov.si/vreme/napovedi%20in%20podatki/radar_anim.gif]] text after")
                .AssertNoErrors();

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (3, par.ChildrenCount);
            fixture.AssertElement<TextElement> (par, 0, x => Assert.AreEqual ("text before", x.Text));
            fixture.AssertElement<ImageElement> (par, 1, x => Assert.AreEqual ("http://www.arso.gov.si/vreme/napovedi in podatki/radar_anim.gif", x.ImageUrl.ToString ()));
            fixture.AssertElement<TextElement> (par, 2, x => Assert.AreEqual ("text after", x.Text));
        }

        [SetUp]
        public void Setup()
        {
            fixture = new ParsingFixture();
        }

        private ParsingFixture fixture;
    }
}