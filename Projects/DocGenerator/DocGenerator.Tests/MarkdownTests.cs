using MarkdownSharp;
using NUnit.Framework;

namespace DocGenerator.Tests
{
    public class MarkdownTests
    {
        [Test]
        public void Bold ()
        {
            string input = "This is **bold**. This is also __bold__.";
            string expected = "<p>This is <strong>bold</strong>. This is also <strong>bold</strong>.</p>\n";

            string actual = markdown.Transform (input);

            Assert.AreEqual (expected, actual);
        }

        [SetUp]
        public void Setup()
        {
            markdown = new Markdown ();
        }

        private Markdown markdown;
    }
}