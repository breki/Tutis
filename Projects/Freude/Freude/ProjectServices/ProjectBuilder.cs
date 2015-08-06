using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
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

        public IEnumerable<string> ListProjectFiles(FreudeProject project)
        {
            return ListProjectFilesPrivate (project.SourceDir);
        }

        public IEnumerable<string> ListBuiltFiles(FreudeProject project)
        {
            return ListBuiltFilesPrivate (project.BuildDir);
        }

        private IEnumerable<string> ListProjectFilesPrivate(string dir)
        {
            Contract.Requires(dir != null);

            foreach (IFileInformation fileInfo in fileSystem.GetDirectoryFiles(dir))
                yield return fileInfo.FullName;

            foreach (IDirectoryInformation dirInfo in fileSystem.GetDirectorySubdirectories(dir))
            {
                string dirFullName = dirInfo.FullName;
                string dirName = Path.GetFileName(dirFullName);
                if (dirName == null || dirName.StartsWith("_", StringComparison.OrdinalIgnoreCase))
                    break;

                foreach (string fileName in ListBuiltFilesPrivate(dirFullName))
                    yield return fileName;
            }
        }

        private IEnumerable<string> ListBuiltFilesPrivate(string dir)
        {
            Contract.Requires(dir != null);

            foreach (IFileInformation fileInfo in fileSystem.GetDirectoryFiles(dir))
                yield return fileInfo.FullName;

            foreach (IDirectoryInformation dirInfo in fileSystem.GetDirectorySubdirectories(dir))
            {
                foreach (string fileName in ListBuiltFilesPrivate(dirInfo.FullName))
                    yield return fileName;
            }
        }

        private readonly IFileSystem fileSystem;
    }
}