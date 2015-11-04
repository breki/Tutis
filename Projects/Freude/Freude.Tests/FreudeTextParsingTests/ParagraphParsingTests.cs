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
            fixture.AssertText(par, 0, "this is some text");
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
            fixture.AssertText(par, 0, @"this is some text and this too and this too");
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
            fixture.AssertText(par, 0, @"par 1 line 1 par 1 line 2");
            par = fixture.AssertElement<ParagraphElement> (1);
            fixture.AssertText(par, 0, @"par 2 line 1 par 2 line 2");
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
            fixture.AssertText(par, 0, @"par 1 line 1 par 1 line 2");
            par = fixture.AssertElement<ParagraphElement> (1);
            fixture.AssertText(par, 0, @"par 2 line 1 par 2 line 2");
        }

        [Test]
        public void EmptyLineCanHaveWhiteSpaceThatIsIgnored ()
        {
            fixture.Parse (@" par 1 line 1
   par 1 line 2
     
par 2 line 1
par 2 line 2 ")
                .AssertNoErrrors ()
                .AssertChildCount (2);

            var par = fixture.AssertElement<ParagraphElement> (0);
            fixture.AssertText(par, 0, @"par 1 line 1 par 1 line 2");
            par = fixture.AssertElement<ParagraphElement> (1);
            fixture.AssertText(par, 0, @"par 2 line 1 par 2 line 2");
        }

        [Test]
        public void BoldInTheMiddle()
        {
            fixture.Parse("There is something '''bold''' in here")
                .AssertNoErrrors()
                .AssertChildCount(1);
            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual(3, par.Children.Count);
            fixture.AssertText(par, 0, @"There is something ");
            fixture.AssertText(par, 1, @"bold", TextElement.TextStyle.Bold);
            fixture.AssertText(par, 2, @" in here");
        }

        [Test]
        public void BoldAtStart()
        {
            fixture.Parse("''' bold ''' in here")
                .AssertNoErrrors()
                .AssertChildCount(1);
            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual(2, par.Children.Count);
            fixture.AssertText(par, 0, @"bold ", TextElement.TextStyle.Bold);
            fixture.AssertText(par, 1, @" in here");
        }

        [Test]
        public void ItalicInTheMiddle()
        {
            fixture.Parse("There is something ''italic'' in here")
                .AssertNoErrrors()
                .AssertChildCount(1);
            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual(3, par.Children.Count);
            fixture.AssertText(par, 0, @"There is something ");
            fixture.AssertText(par, 1, @"italic", TextElement.TextStyle.Italic);
            fixture.AssertText(par, 2, @" in here");
        }

        [Test]
        public void ItalicAtStart()
        {
            fixture.Parse("'' italic '' in here")
                .AssertNoErrrors()
                .AssertChildCount(1);
            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual(2, par.Children.Count);
            fixture.AssertText(par, 0, @"italic ", TextElement.TextStyle.Italic);
            fixture.AssertText(par, 1, @" in here");
        }

        [SetUp]
        public void Setup ()
        {
            fixture = new ParsingFixture ();
        }

        private ParsingFixture fixture;
    }
}