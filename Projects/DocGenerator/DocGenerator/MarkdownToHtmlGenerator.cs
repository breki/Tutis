using System;
using System.IO;
using System.Runtime.CompilerServices;
using MarkdownSharp;

namespace DocGenerator
{
    public class MarkdownToHtmlGenerator
    {
        public void Generate(
            string inputDir,
            string outputDir,
            string layoutFileName,
            string contentDir)
        {
            PrepareOutputDir(outputDir);

            foreach (string markdownFileName in Directory.GetFiles (inputDir, "*.markdown"))
                ProcessMarkdownFile(markdownFileName, outputDir, layoutFileName, contentDir);

            CopyContentFiles(contentDir, outputDir);
        }

        private static void PrepareOutputDir(string outputDir)
        {
            if (Directory.Exists(outputDir))
                Directory.Delete(outputDir, true);

            Directory.CreateDirectory(outputDir);
        }

        private static void ProcessMarkdownFile(string markdownFileName, string outputDir, string layoutFileName, string cssFileName)
        {
            IMarkdownOptions options = new MarkdownOptions();
            Markdown markdown = new Markdown(options);

            string markdownContent = File.ReadAllText(markdownFileName);
            string htmlFragmentContent = markdown.Transform(markdownContent);

            string layoutContent = File.ReadAllText(layoutFileName);
            string htmlContent = layoutContent.Replace("%%content%%", htmlFragmentContent);

            string outputFileName = Path.Combine(outputDir, Path.ChangeExtension(Path.GetFileNameWithoutExtension(markdownFileName), ".html"));
            File.WriteAllText(outputFileName, htmlContent);
        }

        private static void CopyContentFiles(string contentDir, string outputDir)
        {
            if (!Directory.Exists(contentDir))
                return;

            foreach (string contentFileName in Directory.GetFiles(contentDir))
            {
                File.Copy(contentFileName, Path.Combine(outputDir, Path.GetFileName(contentFileName)));
            }
        }
    }
}