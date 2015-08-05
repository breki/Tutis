using System.Collections.Generic;
using System.IO;
using System.Text;
using Brejc.Common.Console;
using Brejc.Common.FileSystem;
using Freude.Commands;
using Freude.Parsing;
using Freude.Templating;
using NUnit.Framework;
using Rhino.Mocks;
using Syborg.Razor;

namespace Freude.Tests.CommandsTests
{
    public class BuildCommandTests
    {
        [Test]
        public void TestCompilingOfAProject()
        {
            ICompiledRazorTemplate compiledTemplate = MockRepository.GenerateMock<ICompiledRazorTemplate> ();

            AddProjectFile("site.css");
            AddProjectFile("weather.freude");
            fileSystem.Stub(x => x.GetDirectoryFiles(ProjectDir)).Return(projectFiles.ToArray());
            fileSystem.Stub(x => x.GetDirectorySubdirectories(ProjectDir)).Return(new IDirectoryInformation[0]);

            fileSystem.Stub(x => x.ReadFileAsString(TemplateFileName)).Return(TemplateBody);
            fileSystem.Stub(x => x.ReadFileAsString(Path.Combine(ProjectDir, "weather.freude"))).Return(FreudeFileBody);
            templatingEngine.Stub(x => x.CompileTemplate(TemplateBody)).Return(compiledTemplate);
            templatingEngine.Stub(x => x.ExpandTemplate(null, null, null)).IgnoreArguments().Return(ExpandedBody);

            fileSystem.Expect(x => x.CopyFile(Path.Combine(ProjectDir, "site.css"), Path.Combine(BuildDir, "site.css")));
            fileSystem.Expect(x => x.WriteFile(Path.Combine(BuildDir, "weather.html"), ExpandedBody, Encoding.UTF8));

            cmd.ParseArgs(consoleEnv, new[] { ProjectDir, TemplateFileName, BuildDir, ".html" });
            cmd.Execute(consoleEnv);

            fileSystem.VerifyAllExpectations();
        }

        [SetUp]
        public void Setup()
        {
            fileSystem = MockRepository.GenerateMock<IFileSystem>();
            textParser = MockRepository.GenerateMock<IFreudeTextParser>();
            templatingEngine = MockRepository.GenerateMock<IFreudeTemplatingEngine>();
            consoleEnv = new ConsoleShell("x");

            cmd = new BuildCommand(fileSystem, textParser, templatingEngine); 
        }

        private void AddProjectFile(string fileName)
        {
            IFileInformation fileInfo = MockRepository.GenerateStub<IFileInformation>();
            fileInfo.Stub(x => x.FullName).Return(Path.Combine(ProjectDir, fileName));
            projectFiles.Add(fileInfo);
        }

        private BuildCommand cmd;
        private IFileSystem fileSystem;
        private IFreudeTextParser textParser;
        private IFreudeTemplatingEngine templatingEngine;
        private IConsoleEnvironment consoleEnv;
        private List<IFileInformation> projectFiles = new List<IFileInformation>();
        private const string TemplateFileName = "template.cshtml";
        private const string TemplateBody = "body";
        private const string ProjectDir = "projDir";
        private const string BuildDir = "buildDir";
        private const string FreudeFileBody = "freude";
        private const string ExpandedBody = "expanded";
    }
}