using System.IO;
using System.Reflection;
using log4net;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace LucenePlaying
{
    public class FileDocumentsSource : IDocumentsSource
    {
        public const string FileNameField = "FileName";
        public const string ContentsField = "Contents";

        public FileDocumentsSource(string rootDir)
        {
            this.rootDir = rootDir;
        }

        public void AddDocuments(IndexWriter indexWriter)
        {
            ExamineDirectory(indexWriter, rootDir);
        }

        private void ExamineDirectory(IndexWriter indexWriter, string directoryName)
        {
            foreach (string fileName in System.IO.Directory.GetFiles(
                directoryName, "*.cs"))
            {
                string fullPath = Path.GetFullPath(fileName);
                AddDocument(indexWriter, fullPath);
            }

            foreach (string directory in System.IO.Directory.GetDirectories(directoryName))
            {
                string fullPath = Path.GetFullPath(directory);
                ExamineDirectory(indexWriter, fullPath);
            }
        }

        private void AddDocument(IndexWriter indexWriter, string fileName)
        {
            Document doc = new Document();
            doc.Add(new Field(FileNameField, fileName, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(
                new Field(ContentsField, File.ReadAllText(fileName), Field.Store.NO, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            indexWriter.AddDocument(doc);

            log.DebugFormat("Indexing file {0}", fileName);
        }

        private readonly string rootDir;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}