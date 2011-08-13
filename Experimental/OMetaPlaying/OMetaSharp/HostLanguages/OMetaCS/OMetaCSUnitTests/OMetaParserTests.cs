using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using OMetaSharp.OMetaCS;
using OMetaSharp.OMetaCS.UnitTests;

namespace OMetaSharp.OMetaCS.UnitTests
{
    public static class OMetaParserTests
    {
        public static void PerformTests()
        {
            TestFromTo();
            TestSpace();
            TestNameFirst();
            TestNameRest();
            TestEscapedChar();
            TestTSString();
            TestCharacters();
            TestSCharacters();
            TestString();
            TestNumber();
            TestHostExpr();
            TestAtomicHostExpr();
            TestArgs();
            TestApplication();
            TestSemanticAction();
            TestSemanticPredicate();
            TestExpr1();
            TestExpr2();
            TestExpr3();
            TestExpr4();
            TestExpr();
            TestRule();
        }

        private static void TestRule()
        {
            AssertParse(@"EscapedChar = '\\' Character:c -> { ""\\"" + c } | Character:c -> { c }", p => p.Rule, @"[Rule, EscapedChar, False, [var c], [Or, [And, And, [Or, [And, [App, Exactly, ""\\""], [Set, c, [App, Character]], [Act,  ""\\"" + c ]], [And, [Set, c, [App, Character]], [Act,  c ]]]]]]");
        }
                
        private static void AssertFromTo(string input, string from, string to, string expected)
        {
            var l = new OMetaStringList(input);
            var s = new OMetaStream<char>(l);
            var p = new OMetaParser();
            var result = p.Match(
                s,
                p.FromTo,
                new OMetaList<HostExpression>[] {
                from.AsHostExpressionList(),
                to.AsHostExpressionList()});

            Debug.Assert(result.ToString().Equals(expected));
        }

        private static void TestFromTo()
        {
            AssertFromTo("AAAAAAAAAAABBBBBBBBBBCCCCCCC", "A", "C", "C");
            AssertFromTo("AAAAAAAAAAABBBBBBBBBBCCCCCCC", "AA", "CC", "CC");
            AssertFromTo("AAAAAAAAAAABBBBBBBBBBCCCCCCC", "AAA", "CCC", "CCC");
        }

        private static void TestSpace()
        {
            AssertParse(" ", p => p.Space, " ");

            // Shouldn't gobble up more than one space
            AssertParse("  ", p => p.Space, " ");

            AssertParse("/*Hello World!*/", p => p.Space, "*/");
        }

        private static void TestNameFirst()
        {
            AssertParse("_", p => p.NameFirst, "_");
            AssertParse("$", p => p.NameFirst, "$");
            AssertParse("abc", p => p.NameFirst, "a");
            AssertWillNotParse("123", p => p.NameFirst);
        }

        private static void TestNameRest()
        {
            AssertParse("_", p => p.NameRest, "_");
            AssertParse("$", p => p.NameRest, "$");
            AssertParse("abc", p => p.NameRest, "a");
            AssertParse("123", p => p.NameRest, "1");
            AssertWillNotParse("%", p => p.NameRest);

        }

        private static void TestTSName()
        {
            AssertParse("helloWorld", p => p.TSName, "helloWorld");
            AssertParse("_helloWorld123", p => p.TSName, "_helloWorld123");
            AssertParse("$helloWorld123", p => p.TSName, "$helloWorld123");
            AssertParse("_", p => p.TSName, "_");
            AssertParse("__", p => p.TSName, "__");
            AssertParse("$", p => p.TSName, "$");
            AssertParse("$_$", p => p.TSName, "$_$");
        }

        private static void TestEscapedChar()
        {
            AssertParse(@"\n", p => p.EscapedChar, "\n");
            AssertParse("q", p => p.EscapedChar, "q");
        }

        private static void TestTSString()
        {
            AssertParse(@"'Hello World!'", p => p.TSString, "Hello World!");
            AssertParse(@"'\''", p => p.TSString, "'");
            AssertParse(@"'Hello World!\n'", p => p.TSString, "Hello World!\n");
            AssertParse(@"''", p => p.TSString, OMetaList<HostExpression>.Nil.ToString());
            AssertParse(@"'\\'", p => p.TSString, @"\");
        }

        private static void TestCharacters()
        {
            AssertParse(@"``Hello''", p => p.Characters, "[App, Seq, \"Hello\"]");
            AssertParse(@"``Hello World!\n''", p => p.Characters, "[App, Seq, \"Hello World!\\n\"]");
        }

        private static void TestSCharacters()
        {
            AssertParse("\"!\"", p => p.SCharacters, "[App, Token, \"!\"]");
            AssertParse("\"My\tToken\"", p => p.SCharacters, "[App, Token, \"My\\tToken\"]");
        }

        private static void TestString()
        {
            AssertParse("#myString", p => p.String, "[App, Exactly, \"myString\"]");
            AssertParse("`myString", p => p.String, "[App, Exactly, \"myString\"]");
            AssertParse("'myString'", p => p.String, "[App, Exactly, \"myString\"]");
            AssertParse("'\"'", p => p.String, "[App, Exactly, \"\\\"\"]");
        }

        private static void TestNumber()
        {
            AssertParse("0", p => p.Number, "[App, Exactly, 0]");
            AssertParse("1", p => p.Number, "[App, Exactly, 1]");
            AssertParse("123", p => p.Number, "[App, Exactly, 123]");
            AssertParse("-1", p => p.Number, "[App, Exactly, -1]");
            AssertParse("-123", p => p.Number, "[App, Exactly, -123]");
            AssertWillNotParse("--123", p => p.Number);
        }

        private static void TestHostExpr()
        {
            AssertParse("(1 + 2)", p => p.HostExpr, "1 + 2");
            AssertParse("(/*1 + */2)", p => p.HostExpr, "/*1 + */2");
            //AssertWillNotParse("(/*1 + 2)", p => p.HostExpr);
        }

        private static void TestAtomicHostExpr()
        {
            AssertParse("{1 + 2}", p => p.AtomicHostExpr, "1 + 2");
            AssertParse("{/*1 + */2}", p => p.AtomicHostExpr, "/*1 + */2");
            //AssertWillNotParse("{/*1 + 2}", p => p.AtomicHostExpr);
        }

        private static void TestArgs()
        {
            // TODO: More
            AssertParse(
                "((/* Hello OMeta#!*/))",
                p => p.Args,
                "/* Hello OMeta#!*/");

            AssertParse("",
                p => p.Args,
                OMetaList<HostExpression>.Nil.ToString());

        }

        private static void TestApplication()
        {
            AssertParse(
                @"MyFancyRuleName((var x = 2; var y = 3; return x + y;))",
                p => p.Application,
                "[App, MyFancyRuleName, var x = 2; var y = 3; return x + y;]");
        }

        private static void TestSemanticAction()
        {
            AssertParse(
                @"->{//Do some cool C# stuff}",
                p => p.SemAction,
                "[Act, //Do some cool C# stuff]");

            AssertParse(
                @"!{1+2}",
                p => p.SemAction,
                "[Act, 1+2]");
        }

        private static void TestSemanticPredicate()
        {
            AssertParse(
                @"?(a==b)",
                p => p.SemPred,
                "[Pred, a==b]");
        }

        private static void TestExpr1()
        {
            AssertParse(@"  123", p => p.Expr1, "[App, Exactly, 123]");
        }

        private static void TestExpr2()
        {
            AssertParse("~123", p => p.Expr2, "[Not, [App, Exactly, 123]]");
            AssertParse("~  ~ 123", p => p.Expr2, "[Not, [Not, [App, Exactly, 123]]]");
            AssertParse("& 123", p => p.Expr2, "[Lookahead, [App, Exactly, 123]]");
            AssertParse("123", p => p.Expr2, "[App, Exactly, 123]");
        }

        private static void TestExpr3()
        {
            AssertParse("~123+ :xyz", p => p.Expr3, "[Many1, [Not, [App, Exactly, 123]]]");
        }

        private static void TestExpr4()
        {
            AssertParse("xyz+ abc+", p => p.Expr4, "[And, [Many1, [App, xyz]], [Many1, [App, abc]]]");
        }

        private static void TestExpr()
        {
            AssertParse("123 | 456 | 789", p => p.Expr, "[Or, [And, [App, Exactly, 123]], [And, [App, Exactly, 456]], [And, [App, Exactly, 789]]]");
            AssertParse("(1|2)", p => p.Expr, "[Or, [And, [Or, [And, [App, Exactly, 1]], [And, [App, Exactly, 2]]]]]");

            // SMELL: OMeta/JS is [Or, [And]] in its pretty print.
            AssertParse("", p => p.Expr, "[Or, And]");
        }
        
        private static void AssertParse(string inputString, Func<OMetaParser, Rule<char>> ruleFetcher, string expectedResult)
        {
            OMetaCSAssert.ParsesToString<OMetaParser>(inputString, ruleFetcher, expectedResult);
        }

        private static void AssertWillNotParse(string inputString, Func<OMetaParser, Rule<char>> ruleFetcher)
        {
            try
            {
                OMetaCSAssert.ParsesToString<OMetaParser>(inputString, ruleFetcher, "");
                Debug.Fail("Should not have parsed");
            }
            catch (Exception)
            {
                // this is what we want
            }
        }
    }
}
