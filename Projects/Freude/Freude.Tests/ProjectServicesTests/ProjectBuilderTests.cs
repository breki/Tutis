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
            const string BuildDir = "builddir";

            FreudeProject project = new FreudeProject();
            project.BuildDir = BuildDir;

            IFileInformation file1Info = MockRepository.GenerateStub<IFileInformation>();
            file1Info.Stub (x => x.FullName).Return (@"builddir\file1.txt");
            IFileInformation file2Info = MockRepository.GenerateStub<IFileInformation>();
            file2Info.Stub (x => x.FullName).Return (@"builddir\dir1\file2");
            IDirectoryInformation dirInfo = MockRepository.GenerateStub<IDirectoryInformation> ();
            dirInfo.Stub(x => x.FullName).Return(@"builddir\dir1");
            
            fileSystem.Stub(x => x.GetDirectoryFiles(BuildDir)).Return(new[] { file1Info });
            fileSystem.Stub(x => x.GetDirectorySubdirectories(BuildDir)).Return(new[] { dirInfo });
            fileSystem.Stub (x => x.GetDirectoryFiles (BuildDir + @"\dir1")).Return (new[] { file2Info });
            fileSystem.Stub (x => x.GetDirectorySubdirectories (BuildDir + @"\dir1")).Return (new IDirectoryInformation[0]);

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