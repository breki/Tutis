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
            throw new System.NotImplementedException();
        }

        private readonly IFileSystem fileSystem;
    }
}