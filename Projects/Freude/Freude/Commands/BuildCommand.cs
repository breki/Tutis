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
using Syborg.Razor;

namespace Freude.Commands
{
    public class BuildCommand : StandardConsoleCommandBase
    {
        public const string BuildMarkerFileName = "freude-build.txt";

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

            AddArg ("project source dir", "path to the project source directory").Value ((x, env) => projectSourceDirectory = x);
            AddArg ("template file", "path to the template file").Value ((x, env) => templateFileName = x);
            AddArg ("build dir", "path to the destination directory where the project will be built").Value ((x, env) => buildDirectory = x);
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
            Contract.Assume(projectSourceDirectory != null);

            FreudeProject project = new FreudeProject();
            CompileTemplate(project);

            PrepareBuildDir();
            ProcessDirectory(project, projectSourceDirectory);
            WriteBuildMarkerFile();

            return 0;
        }

        private void PrepareBuildDir()
        {
            if (fileSystem.DoesFileExist(Path.Combine(buildDirectory, BuildMarkerFileName)))
                fileSystem.DeleteDirectory(buildDirectory);

            fileSystem.EnsureDirectoryExists(buildDirectory);
        }

        private void CompileTemplate(FreudeProject project)
        {
            string templateBody = ReadTemplateFile();
            ICompiledRazorTemplate compiledTemplate = freudeTemplatingEngine.CompileTemplate(templateBody);
            project.RegisterTemplate("default", compiledTemplate);
        }

        private string ReadTemplateFile()
        {
            return fileSystem.ReadFileAsString(templateFileName);
        }

        private void ProcessDirectory(FreudeProject project, string sourceDirectory)
        {
            foreach (IFileInformation fileInfo in fileSystem.GetDirectoryFiles (sourceDirectory))
            {
                string fileName = fileInfo.FullName;
                if (Path.GetExtension(fileName) == ".freude")
                    ProcessFreudeFile(project, fileName);
                else
                    CopyFileToBuildDir(fileName);
            }

            Contract.Assume (projectSourceDirectory != null);

            foreach (IDirectoryInformation dirInfo in fileSystem.GetDirectorySubdirectories (sourceDirectory))
                ProcessDirectory (project, dirInfo.FullName);
        }

        private void WriteBuildMarkerFile()
        {
            fileSystem.WriteFile(Path.Combine(buildDirectory, BuildMarkerFileName), string.Empty, Encoding.UTF8);
        }

        private void ProcessFreudeFile(FreudeProject project, string fileName)
        {
            string freudeText = fileSystem.ReadFileAsString(fileName);
            DocumentDef doc = freudeTextParser.ParseText(freudeText);

            ICompiledRazorTemplate template = project.GetTemplate("default");
            string expandedBody = freudeTemplatingEngine.ExpandTemplate(template, doc, project);

            string destinationFileName = ConstructDestinationFileName (fileName);
            destinationFileName = Path.ChangeExtension(destinationFileName, expandedFileExtension);

            fileSystem.EnsureDirectoryExists(Path.GetDirectoryName(destinationFileName));
            fileSystem.WriteFile(destinationFileName, expandedBody, Encoding.UTF8);
        }

        private void CopyFileToBuildDir(string fileName)
        {
            string destinationFileName = ConstructDestinationFileName(fileName);

            fileSystem.EnsureDirectoryExists (Path.GetDirectoryName (destinationFileName));
            fileSystem.CopyFile (fileName, destinationFileName);
            log.InfoFormat("Copied file '{0}' to '{1}'", fileName, destinationFileName);
        }

        private string ConstructDestinationFileName(string fileName)
        {
            PathBuilder filePath = new PathBuilder (projectSourceDirectory).DebasePath(fileName, false);
            string destinationFileName = Path.Combine(buildDirectory, filePath.ToString());
            return destinationFileName;
        }

        private string templateFileName;
        private string projectSourceDirectory;
        private string buildDirectory;
        private string expandedFileExtension;
        private readonly IFileSystem fileSystem;
        private readonly IFreudeTextParser freudeTextParser;
        private readonly IFreudeTemplatingEngine freudeTemplatingEngine;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}