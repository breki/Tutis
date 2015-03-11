using System;
using System.IO;
using MarkdownSharp;

namespace DocGenerator
{
    public class MarkdownToHtmlGenerator
    {
        public void Generate(
            string inputDir,
            string outputDir,
            string layoutFileName,
            string cssFileName)
        {
            foreach (string markdownFileName in Directory.GetFiles (inputDir, "*.markdown"))
                ProcessMarkdownFile(markdownFileName, outputDir, layoutFileName, cssFileName);
        }

        private static void ProcessMarkdownFile(string markdownFileName, string outputDir, string layoutFileName, string cssFileName)
        {
            IMarkdownOptions options = new MarkdownOptions();
            Markdown markdown = new Markdown(options);

            string markdownContent = File.ReadAllText(markdownFileName);
            string htmlContent = markdown.Transform(markdownContent);

            string outputFileName = Path.Combine(outputDir, Path.ChangeExtension(Path.GetFileNameWithoutExtension(markdownFileName), ".html"));
            File.WriteAllText(outputFileName, htmlContent);
        }
    }
}