using System;
using Brejc.Common;
using Freude.DocModel;
using NUnit.Framework;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class IndentedParagraphParsingTests
    {
        [Test]
        public void SimpleCase()
        {
            fixture.Parse (": something here")
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (ParagraphElement.ParagraphType.Regular, par.Type);
            Assert.AreEqual (1, par.Indentation);
            Assert.AreEqual (1, par.ChildrenCount);
            fixture.AssertText(par, 0, "something here", TextElement.TextStyle.Regular);
        }

        [Test]
        public void Indented()
        {
            fixture.Parse ("::: something here")
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (ParagraphElement.ParagraphType.Regular, par.Type);
            Assert.AreEqual (3, par.Indentation);
            Assert.AreEqual (1, par.ChildrenCount);
            fixture.AssertText(par, 0, "something here", TextElement.TextStyle.Regular);
        }

        [Test]
        public void WithColons()
        {
            fixture.Parse (": something :is: here")
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (ParagraphElement.ParagraphType.Regular, par.Type);
            Assert.AreEqual (1, par.ChildrenCount);
            fixture.AssertText (par, 0, "something :is: here", TextElement.TextStyle.Regular);
        }

        [Test]
        public void WithStyling ()
        {
            fixture.Parse (": something '''here'''")
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (ParagraphElement.ParagraphType.Regular, par.Type);
            Assert.AreEqual (2, par.ChildrenCount);
            fixture.AssertText (par, 0, "something ", TextElement.TextStyle.Regular);
            fixture.AssertText (par, 1, "here", TextElement.TextStyle.Bold);
        }

        [Test]
        public void WithExtraSpacing ()
        {
            fixture.Parse (":   something here  ")
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (ParagraphElement.ParagraphType.Regular, par.Type);
            Assert.AreEqual (1, par.ChildrenCount);
            fixture.AssertText (par, 0, "something here", TextElement.TextStyle.Regular);
        }

        [Test]
        public void Multiline()
        {
            fixture.Parse (":   something here{0}and there".Fmt(Environment.NewLine))
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (ParagraphElement.ParagraphType.Regular, par.Type);
            Assert.AreEqual (1, par.ChildrenCount);
            fixture.AssertText (par, 0, "something here and there", TextElement.TextStyle.Regular);
        }

        [SetUp]
        public void Setup ()
        {
            fixture = new ParsingFixture ();
        }

        private ParsingFixture fixture;
    }
}