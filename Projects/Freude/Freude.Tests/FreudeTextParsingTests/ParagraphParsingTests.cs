using System;
using Brejc.Common;
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
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            fixture.AssertText(par, 0, "this is some text");
        }

        [Test]
        public void MultilineTextParagraphIsMergedIntoSingleLineText ()
        {
            fixture.Parse (@"this is some text
   and this too
and this: too ")
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            fixture.AssertText(par, 0, @"this is some text and this too and this: too");
        }

        [Test]
        public void ParagraphsAreSplitByEmptyLines ()
        {
            fixture.Parse (@" par 1 line 1
   par 1 line 2

par 2 line 1
par 2 line 2 ")
                .AssertNoErrors ()
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
                .AssertNoErrors ()
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
                .AssertNoErrors ()
                .AssertChildCount (2);

            var par = fixture.AssertElement<ParagraphElement> (0);
            fixture.AssertText(par, 0, @"par 1 line 1 par 1 line 2");
            par = fixture.AssertElement<ParagraphElement> (1);
            fixture.AssertText(par, 0, @"par 2 line 1 par 2 line 2");
        }

        [Test] 
        public void NewLineBetweenDifferentStyledTextElementsShouldBecomeSpace1()
        {
            fixture.Parse (@"'''line 1'''
line 2")
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual(2, par.ChildrenCount);
            fixture.AssertText (par, 0, @"line 1");
            fixture.AssertText (par, 1, @" line 2");
        }

        [Test] 
        public void NewLineBetweenDifferentStyledTextElementsShouldBecomeSpace2()
        {
            fixture.Parse (@"line 1
'''line 2'''")
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual(2, par.ChildrenCount);
            fixture.AssertText (par, 0, @"line 1");
            fixture.AssertText (par, 1, @" line 2");
        }

        [Test]
        public void BoldInTheMiddle()
        {
            fixture.Parse("There is something '''bold''' in here")
                .AssertNoErrors()
                .AssertChildCount(1);
            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual(3, par.ChildrenCount);
            fixture.AssertText(par, 0, @"There is something ");
            fixture.AssertText(par, 1, @"bold", TextElement.TextStyle.Bold);
            fixture.AssertText(par, 2, @" in here");
        }

        [Test]
        public void BoldAtStart()
        {
            fixture.Parse("''' bold ''' in here")
                .AssertNoErrors()
                .AssertChildCount(1);
            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual(2, par.ChildrenCount);
            fixture.AssertText(par, 0, @"bold ", TextElement.TextStyle.Bold);
            fixture.AssertText(par, 1, @" in here");
        }

        [Test]
        public void ItalicInTheMiddle()
        {
            fixture.Parse("There is something ''italic'' in here")
                .AssertNoErrors()
                .AssertChildCount(1);
            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual(3, par.ChildrenCount);
            fixture.AssertText(par, 0, @"There is something ");
            fixture.AssertText(par, 1, @"italic", TextElement.TextStyle.Italic);
            fixture.AssertText(par, 2, @" in here");
        }

        [Test]
        public void ItalicAtStart()
        {
            fixture.Parse("'' italic '' in here")
                .AssertNoErrors()
                .AssertChildCount(1);
            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual(2, par.ChildrenCount);
            fixture.AssertText(par, 0, @"italic ", TextElement.TextStyle.Italic);
            fixture.AssertText(par, 1, @" in here");
        }

        [Test]
        public void BoldItalicCombo ()
        {
            fixture.Parse ("There is something '''''boldly italic'' in here.''' Yes!")
                .AssertNoErrors ()
                .AssertChildCount (1);
            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (4, par.ChildrenCount);
            fixture.AssertText (par, 0, @"There is something ", TextElement.TextStyle.Regular);
            fixture.AssertText (par, 1, @"boldly italic", TextElement.TextStyle.BoldItalic);
            fixture.AssertText (par, 2, @" in here.", TextElement.TextStyle.Bold);
            fixture.AssertText (par, 3, @" Yes!", TextElement.TextStyle.Regular);
        }

        [Test]
        public void MultilineStyling()  
        {
            fixture.Parse("There is something '''bold{0}and newline''' in here".Fmt(Environment.NewLine))
                .AssertNoErrors()
                .AssertChildCount(1);
            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual(3, par.ChildrenCount);
            fixture.AssertText(par, 0, @"There is something ");
            fixture.AssertText(par, 1, @"bold and newline", TextElement.TextStyle.Bold);
            fixture.AssertText(par, 2, @" in here");
        }

        [SetUp]
        public void Setup ()
        {
            fixture = new ParsingFixture ();
        }

        private ParsingFixture fixture;
    }
}