using Freude.DocModel;
using NUnit.Framework;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class ParagraphParsingTests
    {
        [Test]
        public void SingleLineTextParagraph ()
        {
            fixture.Parse (@" this is some text ")
                .AssertNoErrrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            fixture.AssertElement<TextElement> (par, 0, x => Assert.AreEqual ("this is some text", x.Text));
        }

        [Test]
        public void MultilineTextParagraphIsMergedIntoSingleLineText ()
        {
            fixture.Parse (@"this is some text
   and this too
and this too ")
                .AssertNoErrrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            fixture.AssertElement<TextElement> (par, 0, x => Assert.AreEqual (@"this is some text and this too and this too", x.Text));
        }

        [Test]
        public void ParagraphsAreSplitByEmptyLines ()
        {
            fixture.Parse (@" par 1 line 1
   par 1 line 2

par 2 line 1
par 2 line 2 ")
                .AssertNoErrrors ()
                .AssertChildCount (2);

            var par = fixture.AssertElement<ParagraphElement> (0);
            fixture.AssertElement<TextElement> (par, 0, x => Assert.AreEqual (@"par 1 line 1 par 1 line 2", x.Text));
            par = fixture.AssertElement<ParagraphElement> (1);
            fixture.AssertElement<TextElement> (par, 0, x => Assert.AreEqual (@"par 2 line 1 par 2 line 2", x.Text));
        }

        [Test]
        public void MultipleEmptyLinesBetweenParagraphsAreTreatedAsSingleOne ()
        {
            fixture.Parse (@" par 1 line 1
   par 1 line 2

   

par 2 line 1
par 2 line 2 ")
                .AssertNoErrrors ()
                .AssertChildCount (2);

            var par = fixture.AssertElement<ParagraphElement> (0);
            fixture.AssertElement<TextElement> (par, 0, x => Assert.AreEqual (@"par 1 line 1 par 1 line 2", x.Text));
            par = fixture.AssertElement<ParagraphElement> (1);
            fixture.AssertElement<TextElement> (par, 0, x => Assert.AreEqual (@"par 2 line 1 par 2 line 2", x.Text));
        }

        [Test]
        public void EmptyLineCanHaveWhitespaceThatIsIgnored ()
        {
            fixture.Parse (@" par 1 line 1
   par 1 line 2
     
par 2 line 1
par 2 line 2 ")
                .AssertNoErrrors ()
                .AssertChildCount (2);

            var par = fixture.AssertElement<ParagraphElement> (0);
            fixture.AssertElement<TextElement> (par, 0, x => Assert.AreEqual (@"par 1 line 1 par 1 line 2", x.Text));
            par = fixture.AssertElement<ParagraphElement> (1);
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