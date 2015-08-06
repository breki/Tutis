using System.IO;
using System.Text;
using Brejc.Common.Console;
using Brejc.Common.FileSystem;
using Freude.Commands;
using Freude.HtmlGenerating;
using Freude.Parsing;
using Freude.ProjectServices;
using Freude.Templating;
using NUnit.Framework;
using Rhino.Mocks;
using Syborg.Razor;

namespace Freude.Tests.CommandsTests
{
    public class BuildCommandTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void TestCompilingOfAProject(bool withBuildDirDeletion)
        {
            ICompiledRazorTemplate compiledTemplate = MockRepository.GenerateMock<ICompiledRazorTemplate> ();

            projectBuilder.Stub(x => x.ListProjectFiles(null))
                .IgnoreArguments ().Return (new[] { @"projDir\weather.freude", @"projDir\site.css", @"projDir\content\other.css" });

            fileSystem.Stub(x => x.ReadFileAsString(TemplateFileName)).Return(TemplateBody);
            fileSystem.Stub(x => x.ReadFileAsString(Path.Combine(ProjectDir, "weather.freude"))).Return(FreudeFileBody);
            templatingEngine.Stub(x => x.CompileTemplate(TemplateBody)).Return(compiledTemplate);
            templatingEngine.Stub(x => x.ExpandTemplate(null, null, null, null)).IgnoreArguments().Return(ExpandedBody);

            if (withBuildDirDeletion)
            {
                fileSystem.Stub(x => x.DoesFileExist(Path.Combine(BuildDir, BuildCommand.BuildMarkerFileName)))
                    .Return(true);
                fileSystem.Expect(x => x.DeleteDirectory(BuildDir)).Repeat.Once();
            }
            else
                fileSystem.Expect(x => x.DeleteDirectory(BuildDir)).Repeat.Never();

            fileSystem.Expect(x => x.CopyFile(Path.Combine(ProjectDir, "site.css"), Path.Combine(BuildDir, "site.css")));
            // a file in a subdirectory should also be copied
            fileSystem.Expect (x => x.CopyFile(Path.Combine (ProjectDir, @"content\other.css"), Path.Combine (BuildDir, @"content\other.css")));
            // subdirectories staring with underscore should be ignored
            fileSystem.Expect (x => x.CopyFile(Path.Combine(ProjectDir, @"_templates\template.cshtml"), Path.Combine (BuildDir, @"_templates\template.cshtml")))
                .Repeat.Never();
            fileSystem.Expect(x => x.WriteFile(Path.Combine(BuildDir, "weather.html"), ExpandedBody, Encoding.UTF8));
            fileSystem.Expect(x => x.WriteFile(Path.Combine(BuildDir, BuildCommand.BuildMarkerFileName), string.Empty, Encoding.UTF8));

            cmd.ParseArgs(consoleEnv, new[] { ProjectDir, TemplateFileName, BuildDir, ".html" });
            cmd.Execute(consoleEnv);

            fileSystem.VerifyAllExpectations();
        }

        [SetUp]
        public void Setup()
        {
            fileSystem = MockRepository.GenerateMock<IFileSystem>();
            projectBuilder = MockRepository.GenerateMock<IProjectBuilder>();
            textParser = MockRepository.GenerateMock<IFreudeTextParser>();
            htmlGenerator = MockRepository.GenerateStub<IHtmlGenerator>();
            templatingEngine = MockRepository.GenerateMock<IFreudeTemplatingEngine>();
            consoleEnv = new ConsoleShell("x");

            cmd = new BuildCommand(fileSystem, projectBuilder, textParser, htmlGenerator, templatingEngine); 
        }

        private BuildCommand cmd;
        private IFileSystem fileSystem;
        private IFreudeTextParser textParser;
        private IFreudeTemplatingEngine templatingEngine;
        private IConsoleEnvironment consoleEnv;
        private IProjectBuilder projectBuilder;
        private IHtmlGenerator htmlGenerator;
        private const string TemplateFileName = "template.cshtml";
        private const string TemplateBody = "body";
        private const string ProjectDir = "projDir";
        private const string BuildDir = "buildDir";
        private const string FreudeFileBody = "freude";
        private const string ExpandedBody = "expanded";
    }
}