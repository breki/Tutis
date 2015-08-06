using System.IO;
using Brejc.Common.FileSystem;
using Freude.DocModel;
using Freude.ProjectServices;
using NUnit.Framework;
using Rhino.Mocks;

namespace Freude.Tests.ProjectServicesTests
{
    public class ProjectBuilderTests
    {
        [Test]
        public void ListProjectFiles()
        {
            const string ProjectDir = "projDir";
            FreudeProject project = new FreudeProject ();
            project.SourceDir = ProjectDir;

            FileStructureMocker fileStructure = new FileStructureMocker ();
            fileStructure.AddFile (Path.Combine (ProjectDir, @"site.css"));
            fileStructure.AddFile (Path.Combine (ProjectDir, @"weather.freude"));
            fileStructure.AddFile (Path.Combine (ProjectDir, @"content\other.css"));
            fileStructure.AddFile (Path.Combine (ProjectDir, @"_templates\template.cshtml"));
            fileStructure.Mock (fileSystem);

            CollectionAssert.AreEquivalent (
                new[] { @"projDir\site.css", @"projDir\weather.freude", @"projDir\content\other.css" }, 
                builder.ListProjectFiles (project));
        }

        [Test] 
        public void ListBuiltFiles()
        {
            const string BuildDir = "builddir";

            FreudeProject project = new FreudeProject();
            project.BuildDir = BuildDir;

            FileStructureMocker fileStructure = new FileStructureMocker ();
            fileStructure.AddFile (Path.Combine (BuildDir, @"file1.txt"));
            fileStructure.AddFile (Path.Combine (BuildDir, @"dir1\file2"));
            fileStructure.Mock (fileSystem);

            CollectionAssert.AreEquivalent(new[] { @"builddir\file1.txt", @"builddir\dir1\file2" }, builder.ListBuiltFiles(project));
        }

        [SetUp]
        public void Setup()
        {
            fileSystem = MockRepository.GenerateStub<IFileSystem>();
            builder = new ProjectBuilder(fileSystem);
        }

        private ProjectBuilder builder;
        private IFileSystem fileSystem;
    }
}