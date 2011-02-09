using System.Diagnostics;
using System.IO;
using System;
using System.Reflection;

namespace OMetaSharp
{
    public static class Grammars
    {
        public static string Find<TGrammar>()
        {
            // Assume that there is a file corresponding to the name of the grammar
            var grammarName = typeof(TGrammar).Name;
            var grammarSearchPattern = grammarName + ".ometa*";
            var matchingFiles = Directory.GetFiles(ProjectPaths.Base, grammarSearchPattern);

            if (matchingFiles.Length == 0)
            {
                // HACK: Try one more..
                var callingAssembly = GrammarResolutionAssembly;
                matchingFiles = Directory.GetFiles(ProjectPaths.GetBase(callingAssembly), grammarSearchPattern);

                if (matchingFiles.Length == 0)
                {
                    // Oh, give up
                    throw new FileNotFoundException();
                }
            }

            Debug.Assert(matchingFiles.Length == 1);
            return matchingFiles[0];
        }

        public static string GetGrammarContents<TGrammar>()
        {
            return File.ReadAllText(Find<TGrammar>());
        }

        public static OMetaList<HostExpression> ParseWith<TParser, TParserInput>(OMetaStream<TParserInput> inputStream, 
                                                                                 Func<TParser, Rule<TParserInput>> ruleFetcher) where TParser : OMeta<TParserInput>, new()
        {
            TParser parser = new TParser();

            var topRule = ruleFetcher(parser);
            Debug.Assert(topRule != null);

            return parser.Match(inputStream, topRule);
        }
                
        public static OMetaList<HostExpression> ParseWith<TParser>(string input, Func<TParser, Rule<char>> ruleFetcher) where TParser : OMeta<char>,new() 
        {
            return Grammars.ParseWith<TParser, char>(new StringStream(input), ruleFetcher);
        }

        public static OMetaList<HostExpression> ParseWith<TParser>(string input, Func<TParser, Rule<char>> ruleFetcher, out OMetaStream<char> modifiedStream) where TParser : OMeta<char>, new()
        {
            var inputStream = new StringStream(input);
            TParser parser = new TParser();

            var topRule = ruleFetcher(parser);
            Debug.Assert(topRule != null);

            return parser.Match(inputStream, topRule, out modifiedStream);
        }

        public static OMetaList<HostExpression> ParseGrammarWith<TGrammar, TParser>(Func<TParser, Rule<char>> ruleFetcher) where TParser : OMeta<char>, new() 
        {
            var grammarPath = Grammars.Find<TGrammar>();

            Debug.Assert(File.Exists(grammarPath));
            string grammarContents = File.ReadAllText(grammarPath);
            return ParseWith<TParser>(grammarContents, ruleFetcher);
        }       

        public static string ParseGrammarThenOptimizeThenTranslate<TGrammar,
                                                                   TParser,
                                                                   TOptimizer,
                                                                   TTranslator>
            (Func<TParser, Rule<char>> topParserRuleFetcher,
             Func<TOptimizer, Rule<HostExpression>> topOptimizerRuleFetcher,
             Func<TTranslator, Rule<HostExpression>> topTranslatorRuleFetcher)
            where TParser : OMeta<char>, new()
            where TOptimizer : Parser<HostExpression>, new()
            where TTranslator : Parser<HostExpression>, new()
        {
            return ParseGrammarThenOptimizeThenTranslate<TParser, 
                                                         TOptimizer, 
                                                         TTranslator>
                (GetGrammarContents<TGrammar>(), 
                 topParserRuleFetcher, 
                 topOptimizerRuleFetcher, 
                 topTranslatorRuleFetcher);
        }

        public static string ParseGrammarThenOptimizeThenTranslate<TParser,
                                                                   TOptimizer,
                                                                   TTranslator>
            (string grammarContents,
             Func<TParser, Rule<char>> topParserRuleFetcher,
             Func<TOptimizer, Rule<HostExpression>> topOptimizerRuleFetcher,
             Func<TTranslator, Rule<HostExpression>> topTranslatorRuleFetcher)
            where TParser : OMeta<char>, new()
            where TOptimizer : Parser<HostExpression>, new()
            where TTranslator : Parser<HostExpression>, new()
        {
            var parsedGrammarList = Grammars.ParseWith<TParser>(grammarContents, topParserRuleFetcher);
            var parsedGrammarStream = new OMetaStream<HostExpression>(new OMetaList<HostExpression>(parsedGrammarList));
            var optimizedGrammar = Grammars.ParseWith<TOptimizer, HostExpression>(parsedGrammarStream, topOptimizerRuleFetcher);

            var optimizedGrammarStream = new OMetaStream<HostExpression>(new OMetaList<HostExpression>(optimizedGrammar));
            var translator = new TTranslator();

            var translatedCode = Grammars.ParseWith<TTranslator, HostExpression>(optimizedGrammarStream, topTranslatorRuleFetcher);

            var generatedCodeString = translatedCode.As<CodeGen>().ToGeneratedCodeString();

            return generatedCodeString;
        }

        public static void WriteGeneratedCode<TGrammar>(string generatedCode, string fileExtension)
        {
            string path = Grammars.Find<TGrammar>();

            string rootDirectory = Path.GetDirectoryName(path);
            if (Path.GetFileName(rootDirectory).Equals("Grammars", StringComparison.OrdinalIgnoreCase))
            {
                rootDirectory = Path.GetDirectoryName(rootDirectory);
            }

            string baseFileName = Path.GetFileNameWithoutExtension(path) + fileExtension;

            path = Path.Combine(rootDirectory, @"GeneratedCode\" + baseFileName);

            WriteGeneratedCode(generatedCode, path);
        }

        public static void WriteGeneratedCode(string generatedCodeContents, string fileName)
        {
            File.WriteAllText(fileName, generatedCodeContents);
        }

        // HACK
        public static Assembly GrammarResolutionAssembly { get; set; }
    }    
}
