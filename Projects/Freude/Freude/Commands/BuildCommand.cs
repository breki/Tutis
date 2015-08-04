using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;
using System.Text;
using Brejc.Common.Console;
using Brejc.Common.FileSystem;
using Freude.DocModel;
using Freude.Parsing;
using Freude.Templating;
using log4net;

namespace Freude.Commands
{
    public class BuildCommand : StandardConsoleCommandBase
    {
        public BuildCommand (
            IFileSystem fileSystem, 
            IFreudeTextParser freudeTextParser,
            IFreudeTemplatingEngine freudeTemplatingEngine)
        {
            Contract.Requires(fileSystem != null);
            Contract.Requires(freudeTextParser != null);
            Contract.Requires(freudeTemplatingEngine != null);

            this.fileSystem = fileSystem;
            this.freudeTextParser = freudeTextParser;
            this.freudeTemplatingEngine = freudeTemplatingEngine;

            AddArg ("site source dir", "path to the site source directory").Value ((x, env) => siteSourceDirectory = x);
            AddArg ("template file", "path to the template file").Value ((x, env) => templateFileName = x);
            AddArg ("build dir", "path to the destination directory where the site will be built").Value ((x, env) => buildDirectory = x);
            AddArg ("extension", "the file extension to be used for files expanded from the template").Value ((x, env) => expandedFileExtension = x);
        }

        public override string CommandId
        {
            get { return "build"; }
        }

        public override object Description
        {
            get { return "build the site"; }
        }

        public override int Execute (IConsoleEnvironment env)
        {
            Contract.Assume(buildDirectory != null);
            Contract.Assume(siteSourceDirectory != null);

            string templateBody = ReadTemplateFile();

            fileSystem.DeleteDirectory(buildDirectory);
            ProcessDirectory(siteSourceDirectory, templateBody);

            return 0;
        }

        private string ReadTemplateFile()
        {
            return fileSystem.ReadFileAsString(templateFileName);
        }

        private void ProcessDirectory(string sourceDirectory, string templateBody)
        {
            foreach (IFileInformation fileInfo in fileSystem.GetDirectoryFiles (sourceDirectory))
            {
                string fileName = fileInfo.FullName;
                if (Path.GetExtension(fileName) == ".freude")
                    ProcessFreudeFile(fileName, templateBody);
                else
                    CopyFileToBuildDir(fileName);
            }

            Contract.Assume (siteSourceDirectory != null);

            foreach (IDirectoryInformation dirInfo in fileSystem.GetDirectorySubdirectories (siteSourceDirectory))
                ProcessDirectory (dirInfo.FullName, templateBody);
        }

        private void ProcessFreudeFile(string fileName, string templateBody)
        {
            string freudeText = fileSystem.ReadFileAsString(fileName);
            DocumentDef doc = freudeTextParser.ParseText(freudeText);

            string expandedBody = freudeTemplatingEngine.ExpandTemplate(templateBody, doc);

            string destinationFileName = ConstructDestinatioFileName (fileName);
            destinationFileName = Path.ChangeExtension(destinationFileName, expandedFileExtension);

            fileSystem.WriteFile(destinationFileName, expandedBody, Encoding.UTF8);
        }

        private void CopyFileToBuildDir(string fileName)
        {
            string destinationFileName = ConstructDestinatioFileName(fileName);

            fileSystem.CopyFile(fileName, destinationFileName);
            log.InfoFormat("Copied file '{0}' to '{1}'", fileName, destinationFileName);
        }

        private string ConstructDestinatioFileName(string fileName)
        {
            PathBuilder filePath = new PathBuilder(fileName);
            filePath = filePath.DebasePath(siteSourceDirectory, false);
            string destinationFileName = Path.Combine(buildDirectory, filePath.ToString());
            return destinationFileName;
        }

        private string templateFileName;
        private string siteSourceDirectory;
        private string buildDirectory;
        private string expandedFileExtension;
        private readonly IFileSystem fileSystem;
        private readonly IFreudeTextParser freudeTextParser;
        private readonly IFreudeTemplatingEngine freudeTemplatingEngine;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}