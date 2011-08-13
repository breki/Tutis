using System;
using Lucene.Net.Index;

namespace LucenePlaying
{
    public interface IDocumentsSource
    {
        void AddDocuments(IndexWriter indexWriter);
    }
}