using System.Collections.Generic;
using MbUnit.Framework;

namespace LucenePlaying.Tests
{
    public class CamelCaseTokenBreakerTests
    {
        [Test]
        public void Test1()
        {
            IList<CamelCaseTokenPart> parts = wordBreaker.BreakIntoWords(
                new CamelCaseTokenPart("camelCaseToken_parts", 100));

            int i = 0;
            Assert.AreEqual(4, parts.Count);
            Assert.AreEqual("camel", parts[i].Text);
            Assert.AreEqual(100, parts[i++].Offset);
            Assert.AreEqual("Case", parts[i].Text);
            Assert.AreEqual(105, parts[i++].Offset);
            Assert.AreEqual("Token", parts[i].Text);
            Assert.AreEqual(109, parts[i++].Offset);
            Assert.AreEqual("parts", parts[i].Text);
            Assert.AreEqual(115, parts[i++].Offset);
        }

        [Test]
        public void SingleWord()
        {
            IList<CamelCaseTokenPart> parts = wordBreaker.BreakIntoWords(
                new CamelCaseTokenPart("word", 100));

            int i = 0;
            Assert.AreEqual(1, parts.Count);
            Assert.AreEqual("word", parts[i].Text);
            Assert.AreEqual(100, parts[i++].Offset);
        }

        [Test]
        public void UnderscoreAtTheEnd()
        {
            IList<CamelCaseTokenPart> parts = wordBreaker.BreakIntoWords(
                new CamelCaseTokenPart("parts_", 100));

            int i = 0;
            Assert.AreEqual(1, parts.Count);
            Assert.AreEqual("parts", parts[i].Text);
            Assert.AreEqual(100, parts[i++].Offset);
        }


        [Test]
        public void UnderscoreAtTheBeginning()
        {
            IList<CamelCaseTokenPart> parts = wordBreaker.BreakIntoWords(
                new CamelCaseTokenPart("_parts", 100));

            int i = 0;
            Assert.AreEqual(1, parts.Count);
            Assert.AreEqual("parts", parts[i].Text);
            Assert.AreEqual(101, parts[i++].Offset);
        }

        [SetUp]
        private void Setup()
        {
            wordBreaker = new CamelCaseWordBreaker();
        }

        private CamelCaseWordBreaker wordBreaker;
    }
}