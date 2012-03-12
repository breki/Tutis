using System.IO;

namespace CleanCode.Step5
{
    public class FileHtmlFetcher : IHtmlFetcher
    {
        public string FetchHtml(string source)
        {
            return File.ReadAllText(source);
        }
    }
}