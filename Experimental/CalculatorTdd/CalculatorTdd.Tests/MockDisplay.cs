using System.Collections.Generic;
using NUnit.Framework;

namespace CalculatorTdd.Tests
{
    public class MockDisplay : IDisplay
    {
        public void SetText(string text)
        {
            log.Add(text);
        }

        public void AssertFinalText(string expectedText)
        {
            CollectionAssert.IsNotEmpty(log);
            Assert.AreEqual(expectedText, log[log.Count-1]);
        }

        public void AssertLog(string[] expectedLog)
        {
            CollectionAssert.AreEqual(expectedLog, log);
        }

        private readonly List<string> log = new List<string>();
    }
}