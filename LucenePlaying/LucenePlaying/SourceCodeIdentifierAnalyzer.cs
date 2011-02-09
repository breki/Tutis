using System.Collections;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Util;

namespace LucenePlaying
{
    public class SourceCodeIdentifierAnalyzer : Analyzer
    {
        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            Hashtable stopWords = new Hashtable();
            stopWords.Add("bool", null);
            stopWords.Add("case", null);
            stopWords.Add("class", null);
            stopWords.Add("decimal", null);
            stopWords.Add("double", null);
            stopWords.Add("else", null);
            stopWords.Add("false", null);
            stopWords.Add("float", null);
            stopWords.Add("for", null);
            stopWords.Add("foreach", null);
            stopWords.Add("get", null);
            stopWords.Add("if", null);
            stopWords.Add("in", null);
            stopWords.Add("int", null);
            stopWords.Add("interface", null);
            stopWords.Add("is", null);
            stopWords.Add("namespace", null);
            stopWords.Add("new", null);
            stopWords.Add("null", null);
            stopWords.Add("object", null);
            stopWords.Add("override", null);
            stopWords.Add("private", null);
            stopWords.Add("protected", null);
            stopWords.Add("public", null);
            stopWords.Add("readonly", null);
            stopWords.Add("return", null);
            stopWords.Add("set", null);
            stopWords.Add("static", null);
            stopWords.Add("string", null);
            stopWords.Add("switch", null);
            stopWords.Add("this", null);
            stopWords.Add("throw", null);
            stopWords.Add("true", null);
            stopWords.Add("using", null);
            stopWords.Add("void", null);

            SourceCodeCharTokenizer sourceCodeCharTokenizer = new SourceCodeCharTokenizer(reader);
            CamelCaseTokenFilter camelCaseTokenFilter = new CamelCaseTokenFilter(sourceCodeCharTokenizer);
            StopFilter stopFilter = new StopFilter(false, camelCaseTokenFilter, stopWords);
            return stopFilter;
        }
    }
}