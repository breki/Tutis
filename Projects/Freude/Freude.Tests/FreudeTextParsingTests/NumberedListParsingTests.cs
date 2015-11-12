using Freude.DocModel;
using NUnit.Framework;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class NumberedListParsingTests
    {
        [Test]
        public void SimpleCase()
        {
            fixture.Parse ("# something here")
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (ParagraphElement.ParagraphType.Numbered, par.Type);
            Assert.AreEqual (0, par.Indentation);
            Assert.AreEqual (1, par.ChildrenCount);
            fixture.AssertText(par, 0, "something here", TextElement.TextStyle.Regular);
        }

        [Test]
        public void Indented()
        {
            fixture.Parse ("### something here")
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (ParagraphElement.ParagraphType.Numbered, par.Type);
            Assert.AreEqual (2, par.Indentation);
            Assert.AreEqual (1, par.ChildrenCount);
            fixture.AssertText(par, 0, "something here", TextElement.TextStyle.Regular);
        }

        [Test]
        public void WithBullets()
        {
            fixture.Parse ("# something #is# here")
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (ParagraphElement.ParagraphType.Numbered, par.Type);
            Assert.AreEqual (1, par.ChildrenCount);
            fixture.AssertText (par, 0, "something #is# here", TextElement.TextStyle.Regular);
        }

        [Test]
        public void WithStyling ()
        {
            fixture.Parse ("# something '''here'''")
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (ParagraphElement.ParagraphType.Numbered, par.Type);
            Assert.AreEqual (2, par.ChildrenCount);
            fixture.AssertText (par, 0, "something ", TextElement.TextStyle.Regular);
            fixture.AssertText (par, 1, "here", TextElement.TextStyle.Bold);
        }

        [Test]
        public void WithExtraSpacing ()
        {
            fixture.Parse ("#   something here  ")
                .AssertNoErrors ()
                .AssertChildCount (1);

            var par = fixture.AssertElement<ParagraphElement> (0);
            Assert.AreEqual (ParagraphElement.ParagraphType.Numbered, par.Type);
            Assert.AreEqual (1, par.ChildrenCount);
            fixture.AssertText (par, 0, "something here", TextElement.TextStyle.Regular);
        }

        [SetUp]
        public void Setup ()
        {
            fixture = new ParsingFixture ();
        }

        private ParsingFixture fixture;
    }
}