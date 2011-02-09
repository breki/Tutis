using System.IO;
using log4net;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using MbUnit.Framework;
using Directory=Lucene.Net.Store.Directory;
using Version=Lucene.Net.Util.Version;

namespace LucenePlaying.Tests
{
    public class SourceCodeIndexingTests
    {
        public const string FileNameField = "FileName";
        public const string ContentsField = "Contents";

        [Test]
        public void CreateIndex()
        {
            Directory directory = FSDirectory.Open(new DirectoryInfo("LuceneIndex"));
            Analyzer analyzer = CreateAnalyzer();
            IndexWriter writer = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED);

            FileDocumentsSource source = new FileDocumentsSource(@"D:\MyStuff\projects");
            source.AddDocuments(writer);

            writer.Optimize();
            writer.Close();

            directory.Close();
        }

        [Test]
        public void InspectIndex()
        {
            Directory directory = FSDirectory.Open(new DirectoryInfo("LuceneIndex"));
            IndexReader indexReader = IndexReader.Open(directory);

            Document doc = indexReader.Document(6);

            indexReader.Close();
            directory.Close();
        }

        [Test]
        public void Search()
        {
            Analyzer analyzer = CreateAnalyzer();
            QueryParser parser = new QueryParser(Version.LUCENE_29, ContentsField, analyzer);
            Query query = parser.Parse("width");
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

        private static Analyzer CreateAnalyzer()
        {
            PerFieldAnalyzerWrapper analyzer = new PerFieldAnalyzerWrapper(
                new SimpleAnalyzer());
            analyzer.AddAnalyzer(ContentsField, new SourceCodeIdentifierAnalyzer());
            return analyzer;            
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(SourceCodeIndexingTests));
    }
}