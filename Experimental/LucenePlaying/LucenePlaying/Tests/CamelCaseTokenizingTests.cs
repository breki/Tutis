using System.Collections.Generic;
using System.IO;
using System.Reflection;
using log4net;
using Lucene.Net.Analysis.Tokenattributes;
using MbUnit.Framework;

namespace LucenePlaying.Tests
{
    public class CamelCaseTokenizingTests
    {
        [Test]
        public void Test()
        {
            List<string> tokensFound = new List<string>();

            using (StringReader reader = new StringReader("ICamelCaseTokenizerTests are very_nice"))
            {
                SourceCodeCharTokenizer tokenizer = new SourceCodeCharTokenizer(reader);
                CamelCaseTokenFilter filter = new CamelCaseTokenFilter(tokenizer);
                while (filter.IncrementToken())
                {
                    TermAttribute attribute = (TermAttribute)tokenizer.GetAttribute(typeof(TermAttribute));
                    log.DebugFormat("Token: {0}", attribute.Term());
                    tokensFound.Add(attribute.Term());
                }
            }

            Assert.AreElementsEqual(
                new[] { "ICamelCaseTokenizerTests", "camel", "case", "tokenizer", "tests", "are", "very_nice", "very", "nice" },
                tokensFound);
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}