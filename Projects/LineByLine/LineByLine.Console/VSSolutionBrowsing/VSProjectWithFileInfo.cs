using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace LineByLine.Console.VSSolutionBrowsing
{
    /// <summary>
    /// Holds information about a VisualStudio project.
    /// </summary>
    public class VSProjectWithFileInfo : VSProjectInfo
    {
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid")]
        public VSProjectWithFileInfo(
            VSSolution ownerSolution, 
            Guid projectGuid, 
            string projectName, 
            LocalPath projectFileName, 
            Guid projectTypeGuid) : base (ownerSolution, projectGuid, projectName, projectTypeGuid)
        {
            this.projectFileName = projectFileName;
        }

        /// <summary>
        /// Gets or sets the <see cref="VSProject"/> object holding the detailed information about this VisualStudio
        /// project.
        /// </summary>
        /// <value>The <see cref="VSProject"/> object .</value>
        public VSProject Project { get; set; }

        /// <summary>
        /// Gets the path to the directory where the project file is located.
        /// </summary>
        /// <value>The project directory path.</value>
        public FullPath ProjectDirectoryPath
        {
            get
            {
                return OwnerSolution.SolutionDirectoryPath.CombineWith(ProjectFileName).ParentPath;
            }
        }

        /// <summary>
        /// Gets the name of the project file. The file name is relative to the solution's directory.
        /// </summary>
        /// <remarks>The full path to the project file can be retrieved using the <see cref="ProjectFileNameFull"/>
        /// property.</remarks>
        /// <value>The name of the project file.</value>
        public LocalPath ProjectFileName
        {
            get { return projectFileName; }
        }

        /// <summary>
        /// Gets the full path to the project file.
        /// </summary>
        /// <value>The full path to the project file.</value>
        public FileFullPath ProjectFileNameFull
        {
            get
            {
                return OwnerSolution.SolutionDirectoryPath.CombineWith(ProjectFileName).ToFileFullPath();
            }
        }

        /// <summary>
        /// Gets the output path for a specified VisualStudio project. The output path is relative
        /// to the directory where the project file is located.
        /// </summary>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <returns>
        /// The output path or <c>null</c> if the project is not compatibile.
        /// </returns>
        /// <exception cref="ArgumentException">The method could not extract the data from the project file.</exception>
        public LocalPath GetProjectOutputPath(string buildConfiguration)
        {
            // skip non-C# projects
            if (ProjectTypeGuid != VSProjectType.CSharpProjectType.ProjectTypeGuid)
                return null;

            // find the project configuration
            string condition = string.Format(
                CultureInfo.InvariantCulture,
                "'$(Configuration)|$(Platform)' == '{0}|AnyCPU'", 
                buildConfiguration);
            VSProjectConfiguration projectConfiguration = Project.FindConfiguration(condition);
            if (projectConfiguration == null)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Could not find '{0}' configuration for the project '{1}'.",
                    condition,
                    ProjectName);

                throw new ArgumentException(message);
            }

            if (false == projectConfiguration.Properties.ContainsKey("OutputPath"))
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Missing OutputPath for the '{0}' configuration of the project '{1}'.",
                    buildConfiguration,
                    ProjectName);

                throw new ArgumentException(message);
            }

            return new LocalPath(projectConfiguration.Properties["OutputPath"]);
        }

        public IXPathNavigable OpenProjectFileAsXmlDocument ()
        {
            //if (log.IsDebugEnabled)
            //    log.DebugFormat ("OpenProjectFileAsXmlDocument '{0}'", this.ProjectFileName);

            using (Stream stream = File.Open (ProjectFileNameFull.ToString(), FileMode.Open, FileAccess.Read))
            {
                XmlDocument xmlDoc = new XmlDocument ();
                xmlDoc.Load (stream);
                return xmlDoc;
            }
        }

        public override void Parse (VSSolutionFileParser parser)
        {
            while (true)
            {
                string line = parser.NextLine();

                if (line == null)
                    parser.ThrowParserException ("Unexpected end of solution file.");

                Match endProjectMatch = VSSolution.RegexEndProject.Match(line);

                if (endProjectMatch.Success)
                    break;
            }
        }

        private readonly LocalPath projectFileName;
        public const string MSBuildNamespace = @"http://schemas.microsoft.com/developer/msbuild/2003";
    }
}