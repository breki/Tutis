using Freude.DocModel;
using NUnit.Framework;

namespace Freude.Tests.FreudeTextParsingTests
{
    public class InternalLinkParsingTests
    {
        [Test]
        public static void SimpleLink()
        {
            //fixture.Parse("[[Main Page]]")
            //    .AssertNoErrrors()
            //    .AssertChildCount(1);

            //var link = fixture.AssertElement<InternalLinkElement>(0);
            //Assert.AreEqual("Main Page", link.Address);
        }

        [SetUp]
        public void Setup ()
        {
            fixture = new ParsingFixture ();
        }

        private ParsingFixture fixture;
    }
}