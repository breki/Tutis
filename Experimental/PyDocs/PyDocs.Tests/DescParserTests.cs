using MbUnit.Framework;
using PyDocs.Descriptions;
using PyDocs.Parsing;

namespace PyDocs.Tests
{
    public class DescParserTests
    {
        [Test]
        public void ParseSample()
        {
            IDescParser parser = new DescParser();
            PackageDesc desc = parser.Parse(@"..\..\..\SampleDocs");
            Assert.AreEqual("SamplePackage", desc.Name);
        }
    }
}