using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMetaSharp.UnitTests;

namespace OMetaSharp.OMetaCS.UnitTests
{
    public class OMetaCSAssert
    {
        public static void ParsesToString<TParser>(string inputString, Func<TParser, Rule<char>> ruleFetcher, string expectedResult) where TParser : OMeta<char>, new()
        {
            var parseResult = Grammars.ParseWith<TParser>(inputString, ruleFetcher);
            var resultString = parseResult.ToString();
            Assert.IsTrue(resultString.Equals(expectedResult, StringComparison.Ordinal));
        }

        public static void ParsesToString<TParser>(string inputString, Func<TParser, Rule<char>> ruleFetcher, string expectedResult, out OMetaStream<char> modifiedStream) where TParser : OMeta<char>, new()
        {
            var parseResult = Grammars.ParseWith<TParser>(inputString, ruleFetcher, out modifiedStream);
            var resultString = parseResult.ToString();
            Assert.IsTrue(resultString.Equals(expectedResult, StringComparison.Ordinal));
        }

        public static void ParsesTo<TParser, TResult>(string inputString, Func<TParser, Rule<char>> ruleFetcher, TResult expected) where TParser : OMeta<char>, new()
        {
            TResult result = Grammars.ParseWith<TParser>(inputString, ruleFetcher).As<TResult>();
            Assert.IsTrue(result.Equals(expected));
        }

        public static void ParsesTo<TParser, TInput, TResult>(TInput input, Func<TParser, Rule<TInput>> ruleFetcher, TResult expected) where TParser : OMeta<TInput>, new()
        {
            var parser = new TParser();
            var s = new OMetaStream<TInput>(input);
            var rule = ruleFetcher(parser);
            var result = parser.Match<TResult>(s, rule);
            Assert.IsTrue(result.Equals(expected));
        }
    }
}
