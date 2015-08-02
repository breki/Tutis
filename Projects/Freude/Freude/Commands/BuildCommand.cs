using System.IO;
using Brejc.Common.Console;
using Brejc.Common.FileSystem;
using Freude.DocModel;
using Freude.Parsing;

namespace Freude.Commands
{
    public class BuildCommand : StandardConsoleCommandBase
    {
        public BuildCommand (
            IFileSystem fileSystem, 
            IFreudeTextParser freudeTextParser)
        {
            this.fileSystem = fileSystem;
            this.freudeTextParser = freudeTextParser;
            AddArg ("site source dir", "path to the site source directory").Value ((x, env) => siteSourceDirectory = x);
            AddArg ("template file", "path to the template file").Value ((x, env) => templateFileName = x);
            AddArg ("build dir", "path to the destination directory where the site will be built").Value ((x, env) => buildDirectory = x);
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
            fileSystem.DeleteDirectory(buildDirectory);
            ProcessDirectory(siteSourceDirectory);

            return 0;
        }

        private void ProcessDirectory(string dir)
        {
            foreach (IFileInformation fileInfo in fileSystem.GetDirectoryFiles(dir))
            {
                string fileName = fileInfo.FullName;
                if (Path.GetExtension(fileName) == ".freude")
                {
                    ProcessFreudeFile(fileName);
                }
                else
                {
                    fileSystem.CopyFile(fileName,);
                }
            }

            foreach (IDirectoryInformation dirInfo in fileSystem.GetDirectorySubdirectories (siteSourceDirectory))
                ProcessDirectory (dirInfo.FullName);
        }

        private void ProcessFreudeFile(string fileName)
        {
            string freudeText = fileSystem.ReadFileAsString(fileName);
            DocumentDef doc = freudeTextParser.ParseText(freudeText);
        }

        private string templateFileName;
        private string siteSourceDirectory;
        private string buildDirectory;
        private readonly IFileSystem fileSystem;
        private readonly IFreudeTextParser freudeTextParser;
    }
}