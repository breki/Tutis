using System.IO;

namespace CleanCode.Step5
{
    public class FileTextFetcher : ITextFetcher
    {
        public string FetchText(string source)
        {
            return File.ReadAllText(source);
        }
    }
}