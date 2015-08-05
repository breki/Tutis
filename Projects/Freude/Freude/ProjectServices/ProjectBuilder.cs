using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Brejc.Common.FileSystem;
using Freude.DocModel;

namespace Freude.ProjectServices
{
    public class ProjectBuilder : IProjectBuilder
    {
        public ProjectBuilder(IFileSystem fileSystem)
        {
            Contract.Requires(fileSystem != null);
            this.fileSystem = fileSystem;
        }

        public IEnumerable<string> ListBuiltFiles(FreudeProject project)
        {
            return ListBuildFilesPrivate (project.BuildDir);
        }

        private IEnumerable<string> ListBuildFilesPrivate(string dir)
        {
            foreach (IFileInformation fileInfo in fileSystem.GetDirectoryFiles(dir))
                yield return fileInfo.FullName;

            foreach (IDirectoryInformation dirInfo in fileSystem.GetDirectorySubdirectories(dir))
            {
                foreach (string fileName in ListBuildFilesPrivate(dirInfo.FullName))
                    yield return fileName;
            }
        }

        private readonly IFileSystem fileSystem;
    }
}