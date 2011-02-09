using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMetaSharp.UnitTests;
using OMetaSharp.OMetaCS;

namespace OMetaSharp.OMetaCS.UnitTests
{
    public static class CSharpRecognizerTests
    {
        public static void PerformTests()
        {
            TestEscapedChar();
            TestLiteralChar();
            TestRegularString();
            TestVerbatimString();
            TestIgnoredChunk();
            TestComment();
            TestParenExpr();
            TestBlock();
            TestSugaryGoodness();
        }
        
        private static void TestEscapedChar()
        {
            ParsesTo(@"\n", p => p.EscapedChar, @"\n");
            ParsesTo(@"\\", p => p.EscapedChar, @"\\");
        }

        private static void TestLiteralChar()
        {
            ParsesTo(@"'\n'", p => p.LiteralChar, @"'\n'");
        }

        private static void TestRegularString()
        {
            ParsesTo("\"Hello World!\"", p => p.RegularString, "\"Hello World!\"");
            ParsesTo("\"\"", p => p.RegularString, "\"\"");
            ParsesTo("\"\\\\\"", p => p.RegularString, "\"\\\\\"");
            ParsesTo("\"\\\"\"", p => p.RegularString, "\"\\\"\"");
        }

        private static void TestVerbatimString()
        {
            ParsesTo("@\"Hello World!\"", p => p.VerbatimString, "@\"Hello World!\"");
            ParsesTo("@\"\"\"\"", p => p.VerbatimString, "@\"\"\"\"");
        }

        private static void TestIgnoredChunk()
        {
            ParsesTo("\"Hello World!\"", p => p.IgnoredChunk, "\"Hello World!\"");
            ParsesTo("\"\"", p => p.IgnoredChunk, "\"\"");
            ParsesTo("\"\\\"\"", p => p.IgnoredChunk, "\"\\\"\"");
            ParsesTo("@\"Hello World!\"", p => p.IgnoredChunk, "@\"Hello World!\"");
            ParsesTo("@\"\"\"\"", p => p.IgnoredChunk, "@\"\"\"\"");
        }

        private static void TestComment()
        {
            ParsesTo("/*This is a comment!*/", p => p.Comment, "/*This is a comment!*/");
            ParsesTo("//This is a comment!\n", p => p.Comment, "//This is a comment!\n");
        }

        private static void TestParenExpr()
        {
            ParsesTo("(\"Hello World\")", p => p.ParenExpr, "\"Hello World\"");
        }

        private static void TestBlock()
        {
            OMetaStream<char> modifiedStream;            
            ParsesTo("{\"Hello World\"}", p => p.Block, "\"Hello World\"");
            OMetaCSAssert.ParsesToString<CSharpRecognizer>("{ xs.As<string>() }Extra", p => p.Block, " xs.As<string>() ", out modifiedStream);
            Assert.AreEqual(modifiedStream.ToString(), "'Extra'");
            OMetaCSAssert.ParsesToString<CSharpRecognizer>("{ Sugar.Implode(\"'\", x, \"'\") }", p => p.Block, " Sugar.Implode(\"'\", x, \"'\") ", out modifiedStream);
            ParsesTo(@"{ ""\\"" + c }", p=>p.Block, @" ""\\"" + c ");
            OMetaCSAssert.ParsesToString<CSharpRecognizer>(@"{ System.Text.RegularExpressions.Regex.Unescape(""\\"" + c.As<string>())[0] } Sample", p => p.Block, @" System.Text.RegularExpressions.Regex.Unescape(""\\"" + c.As<string>())[0] ", out modifiedStream);
            Assert.AreEqual(modifiedStream.ToString(), "' Sample'");
            
            // This doesn't work yet due to lack of spaces working right yet.
            //ParsesTo("{ \"Hello World\" }", p => p.Block, " \"Hello World\" ");
        }

        private static void TestSugaryGoodness()
        {
            ParsesTo("{\"Hello\",\"World\",abc}", p => p.SugaryList, "Sugar.Cons(\"Hello\",\"World\",abc)");
        }

        private static void ParsesTo(string inputString, Func<CSharpRecognizer, Rule<char>> ruleFetcher, string expectedResult)
        {
            OMetaCSAssert.ParsesToString<CSharpRecognizer>(inputString, ruleFetcher, expectedResult);
        }
    }
}
