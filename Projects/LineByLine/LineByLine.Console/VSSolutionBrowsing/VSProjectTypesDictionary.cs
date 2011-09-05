using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LineByLine.Console.VSSolutionBrowsing
{
    /// <summary>
    /// A dictionary of registered VisualStudio project types.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public class VSProjectTypesDictionary
    {
        public VSProjectTypesDictionary()
        {
            // add some common project types
            RegisterProjectType(VSProjectType.SolutionFolderProjectType);
            RegisterProjectType(VSProjectType.CSharpProjectType);
        }

        /// <summary>
        /// Registers a new type of the VisualStudio project.
        /// </summary>
        /// <param name="projectType">><see cref="VSProjectType"/> object to be registered.</param>
        public void RegisterProjectType (VSProjectType projectType)
        {
            projectTypes.Add(projectType.ProjectTypeGuid, projectType);
        }

        /// <summary>
        /// Tries to find <see cref="VSProjectType"/> object for a specific VisualStudio project type Guid.
        /// </summary>
        /// <param name="projectTypeGuid">The project type GUID.</param>
        /// <returns><see cref="VSProjectType"/> object holding information about the specified VisualStudio project 
        /// type; <c>null</c> if the project type is not registered.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid")]
        public VSProjectType FindProjectType(Guid projectTypeGuid)
        {
            if (projectTypes.ContainsKey(projectTypeGuid))
                return projectTypes[projectTypeGuid];

            return null;
        }

        private readonly Dictionary<Guid, VSProjectType> projectTypes = new Dictionary<Guid, VSProjectType>();
    }
}