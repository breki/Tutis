using System;
using MbUnit.Framework;
using PyDocs.Parsing;

namespace PyDocs.Tests
{
    public class BlockParserTests
    {
        [Test]
        public void ReadSampleClassDescFile()
        {
            BlockParser parser = new BlockParser();
            ParsedBlock block = parser.ParseFileIntoBlocks(@"..\..\..\SampleDocs\SampleModule\SampleDesc.txt");

            Assert.IsNotNull(block);

            Assert.AreEqual(1, block.Children.Count);

            ParsedBlock topChild = block.Children[0];
            Assert.AreEqual("c", topChild.BlockType);
            Assert.AreEqual("Test", topChild.BlockName);
            Assert.AreEqual("Defines bla bla." + Environment.NewLine, topChild.BlockContent);
            Assert.AreEqual(1, topChild.Children.Count);
            Assert.AreEqual("m", topChild.Children[0].BlockType);
        }

        [Test]
        public void ReadSamplePackageDescFile()
        {
            BlockParser parser = new BlockParser();
            ParsedBlock block = parser.ParseFileIntoBlocks(@"..\..\..\SampleDocs\__package.txt");

            Assert.IsNotNull(block);

            Assert.AreEqual(1, block.Children.Count);

            ParsedBlock topChild = block.Children[0];
            Assert.AreEqual("package", topChild.BlockType);
            Assert.AreEqual("SamplePackage", topChild.BlockName);
            Assert.AreEqual("SamplePackage description.\r\n\r\nThird line.\r\n", topChild.BlockContent);
        }
    }
}