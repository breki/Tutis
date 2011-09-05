using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace LineByLine.Console.VSSolutionBrowsing
{
    /// <summary>
    /// Represents a VisualStudio solution.
    /// </summary>
    public class VSSolution
    {
        /// <summary>
        /// Gets a read-only collection of <see cref="VSProjectWithFileInfo"/> objects for all of the projects in the solution.
        /// </summary>
        /// <value>A read-only collection of <see cref="VSProjectWithFileInfo"/> objects .</value>
        [SuppressMessage ("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos")]
        public ReadOnlyCollection<VSProjectInfo> Projects
        {
            get { return projects.AsReadOnly(); }
        }

        /// <summary>
        /// Gets or sets the VisualStudio project types dictionary.
        /// </summary>
        /// <value>The VisualStudio project types dictionary.</value>
        public VSProjectTypesDictionary ProjectTypesDictionary
        {
            get { return projectTypesDictionary; }
            set { projectTypesDictionary = value; }
        }

        public FullPath SolutionDirectoryPath
        {
            get { return solutionFileName.Directory; }
        }

        public FileFullPath SolutionFileName
        {
            get { return solutionFileName; }
        }

        public decimal SolutionVersion
        {
            get { return solutionVersion; }
        }

        /// <summary>
        /// Finds the project by its unique id.
        /// </summary>
        /// <param name="projectGuid">The project's GUID.</param>
        /// <returns>The <see cref="VSProjectWithFileInfo"/> object representing the project.</returns>
        /// <exception cref="ArgumentException">The project was not found.</exception>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid")]
        public VSProjectInfo FindProjectById(Guid projectGuid)
        {
            foreach (VSProjectInfo projectData in projects)
                if (projectData.ProjectGuid == projectGuid)
                    return projectData;

            throw new ArgumentException(string.Format(
                CultureInfo.InvariantCulture,
                "Project {0} not found.",
                projectGuid));
        }

        public VSProjectInfo FindProjectByName(string projectName)
        {
            foreach (VSProjectInfo projectData in projects)
                if (projectData.ProjectName == projectName)
                    return projectData;

            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Project {0} not found.", projectName));
        }

        /// <summary>
        /// Performs the specified action on each project of the solution.
        /// </summary>
        /// <param name="action">The action delegate to perform on each project.</param>
        public void ForEachProject (Action<VSProjectInfo> action)
        {
            projects.ForEach(action);
        }

        /// <summary>
        /// Loads the specified VisualStudio solution file and returns a <see cref="VSSolution"/> representing the solution.
        /// </summary>
        /// <param name="fileName">The name of the solution file.</param>
        /// <returns>A <see cref="VSSolution"/> representing the solution.</returns>
        public static VSSolution Load (string fileName)
        {
            VSSolution solution = new VSSolution (fileName);

            using (Stream stream = File.Open (fileName, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader (stream))
                {
                    VSSolutionFileParser parser = new VSSolutionFileParser(reader);

                    string line = parser.NextLine();

                    Match solutionMatch = RegexSolutionVersion.Match (line);

                    if (solutionMatch.Success == false)
                        parser.ThrowParserException("Not a solution file.");

                    solution.solutionVersion = decimal.Parse (
                        solutionMatch.Groups["version"].Value, 
                        CultureInfo.InvariantCulture);

                    while (true)
                    {
                        line = parser.NextLine ();

                        if (line == null)
                            break;

                        // exit the loop when 'Global' section appears
                        Match globalMatch = RegexGlobal.Match (line);
                        if (globalMatch.Success)
                            break;

                        Match projectMatch = RegexProject.Match (line);

                        if (projectMatch.Success == false)
                            parser.ThrowParserException (
                                String.Format (
                                    CultureInfo.InvariantCulture,
                                    "Could not parse solution file (line {0}).", 
                                    parser.LineCount));

                        Guid projectGuid = new Guid (projectMatch.Groups["projectGuid"].Value);
                        string projectName = projectMatch.Groups["name"].Value;
                        string projectFileName = projectMatch.Groups["path"].Value;
                        Guid projectTypeGuid = new Guid (projectMatch.Groups["projectTypeGuid"].Value);

                        VSProjectInfo project;
                        if (projectTypeGuid == VSProjectType.SolutionFolderProjectType.ProjectTypeGuid)
                        {
                            project = new VSSolutionFilesInfo(
                                solution,
                                projectGuid,
                                projectName,
                                projectTypeGuid);
                        }
                        else
                        {
                            project = new VSProjectWithFileInfo(
                                solution,
                                projectGuid,
                                projectName,
                                new LocalPath(projectFileName),
                                projectTypeGuid);
                        }

                        solution.projects.Add (project);
                        project.Parse (parser);
                    }
                }
            }

            return solution;
        }

        /// <summary>
        /// Loads the VisualStudio project files and fills the project data into <see cref="VSProjectWithFileInfo.Project"/> 
        /// properties for each of the project in the solution.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public void LoadProjects()
        {
            ForEachProject(
                delegate (VSProjectInfo projectInfo)
                {
                    if (projectInfo.ProjectTypeGuid == VSProjectType.CSharpProjectType.ProjectTypeGuid)
                        ((VSProjectWithFileInfo)projectInfo).Project = VSProject.Load (((VSProjectWithFileInfo)projectInfo).ProjectFileNameFull.ToString());
                });
        }

        public static readonly Regex RegexEndProject = new Regex (@"^EndProject$");
        public static readonly Regex RegexGlobal = new Regex (@"^Global$");
        public static readonly Regex RegexProject = new Regex (@"^Project\(""(?<projectTypeGuid>.*)""\) = ""(?<name>.*)"", ""(?<path>.*)"", ""(?<projectGuid>.*)""$");
        public static readonly Regex RegexSolutionVersion = new Regex (@"^Microsoft Visual Studio Solution File, Format Version (?<version>.+)$");

        protected VSSolution (string fileName)
        {
            solutionFileName = new FileFullPath(fileName);
        }

        private readonly List<VSProjectInfo> projects = new List<VSProjectInfo> ();
        private VSProjectTypesDictionary projectTypesDictionary = new VSProjectTypesDictionary();
        private readonly FileFullPath solutionFileName;
        private decimal solutionVersion;
    }
}