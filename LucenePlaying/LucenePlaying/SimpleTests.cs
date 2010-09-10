using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MbUnit.Framework;
using Directory=Lucene.Net.Store.Directory;
using Version=Lucene.Net.Util.Version;

namespace LucenePlaying
{
    public class SimpleTests
    {
        public const string FileNameField = "FileName";
        public const string ContentsField = "Contents";

        [Test]
        public void CreateIndex()
        {
            Directory directory = FSDirectory.Open(new DirectoryInfo("LuceneIndex"));
            Analyzer analyzer = CreateAnalyzer();
            IndexWriter writer = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);

            foreach (string fileName in System.IO.Directory.GetFiles(@"..\..\..\..\WinFormsLayout", "*.cs"))
            {
                string fullPath = Path.GetFullPath(fileName);

                Document doc = new Document();
                doc.Add(new Field(FileNameField, fullPath, Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(
                    new Field(ContentsField, File.ReadAllText(fileName), Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
                writer.AddDocument(doc);

                log.DebugFormat("Indexing file {0}", fullPath);
            }

            writer.Optimize();
            writer.Close();

            directory.Close();
        }

        [Test]
        public void Search()
        {
            //Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_29);
            Analyzer analyzer = CreateAnalyzer();
            QueryParser parser = new QueryParser(Version.LUCENE_29, ContentsField, analyzer);
            Query query = parser.Parse("layout");
            Directory directory = FSDirectory.Open(new DirectoryInfo("LuceneIndex"));
            IndexSearcher searcher = new IndexSearcher(directory, false);

            //Filter filter = null;
            //Term term = new Term("FileContents", "Layout");
            //query = new TermQuery(term);
            //query = new PhraseQuery();

            //TopScoreDocCollector collector = new TopScoreDocCollector()
            int freq = searcher.DocFreq(new Term(ContentsField, "layout"));
            TopDocs topDocs = searcher.Search(query, 1000);

            for (int i = 0; i < topDocs.scoreDocs.Length; i++)
            {
                ScoreDoc score = topDocs.scoreDocs[i];
                Document doc = searcher.Doc(score.doc);
                log.DebugFormat(
                    "{0} ({1})",
                    doc.GetField(FileNameField).StringValue(),
                    score.doc);
            }

            //foreach (ScoreDoc scoreDoc in topDocs.scoreDocs)
            //{
            //    log.Debug(scoreDoc);
            //    Document doc = searcher.Doc(scoreDoc.doc);
            //    log.Debug(doc.GetField("FileName").StringValue());
            //}

            searcher.Close();
            directory.Close();

            Assert.IsTrue(topDocs.totalHits > 0);
        }

        [FixtureSetUp]
        public void FixtureSetup()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        private Analyzer CreateAnalyzer()
        {
            PerFieldAnalyzerWrapper analyzer = new PerFieldAnalyzerWrapper(
                new SimpleAnalyzer());
            analyzer.AddAnalyzer(ContentsField, new SourceCodeIdentifierAnalyzer());
            return analyzer;            
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(SimpleTests));
    }

    public class SourceCodeIdentifierAnalyzer : Analyzer
    {
        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            return new CamelCaseTokenizer(reader);
        }
    }

    public class CamelCaseTokenizer : Tokenizer
    {
        public CamelCaseTokenizer(TextReader reader) : base(reader)
        {
            offset = 0;
            bufferIndex = 0;
            dataLen = 0;
            ioBuffer = new char[0x1000];
            offsetAtt = (OffsetAttribute)AddAttribute(typeof(OffsetAttribute));
            termAtt = (TermAttribute)AddAttribute(typeof(TermAttribute));
        }

        public override void End()
        {
            int finalOffset = CorrectOffset(this.offset);
            offsetAtt.SetOffset(finalOffset, finalOffset);
        }

        public override bool IncrementToken()
        {
            ClearAttributes();

            int length = 0;
            int start = bufferIndex;
            char[] buffer = termAtt.TermBuffer();

            while (true)
            {
                if (bufferIndex >= dataLen)
                {
                    offset += dataLen;
                    dataLen = input.Read(ioBuffer, 0, ioBuffer.Length);
                    if (dataLen <= 0)
                    {
                        dataLen = 0;
                        if (length <= 0)
                            return false;

                        break;
                    }

                    bufferIndex = 0;
                }

                char c = ioBuffer[bufferIndex++];
                if (IsTokenChar(c))
                {
                    if (length == 0)
                        start = (offset + bufferIndex) - 1;
                    else if (length == buffer.Length)
                        buffer = termAtt.ResizeTermBuffer(1 + length);
                    buffer[length++] = c;

                    if (length == 0xff)
                        break;
                }
                else if (length > 0)
                    break;
            }

            termAtt.SetTermLength(length);
            offsetAtt.SetOffset(CorrectOffset(start), CorrectOffset(start + length));

            return true;
        }

        private bool IsTokenChar(char c)
        {
            if (char.IsWhiteSpace(c))
                return false;

            return true;
        }

        public override void Reset(TextReader input)
        {
            base.Reset(input);
            bufferIndex = 0;
            offset = 0;
            dataLen = 0;
        }

        private int bufferIndex;
        private int dataLen;
        private const int IO_BUFFER_SIZE = 0x1000;
        private char[] ioBuffer;
        private const int MAX_WORD_LEN = 0xff;
        private int offset;
        private OffsetAttribute offsetAtt;
        private TermAttribute termAtt;
    }
}