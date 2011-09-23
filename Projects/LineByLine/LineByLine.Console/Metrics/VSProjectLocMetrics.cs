using System.IO;
using LineByLine.Console.VSSolutionBrowsing;

namespace LineByLine.Console.Metrics
{
    /// <summary>
    /// Implementation of the GroupLocMetricsBase abstarct class.
    /// Represents a Visual studio project with a list of source
    /// files and their loc metrics.
    /// </summary>
    public class VSProjectLocMetrics : GroupLocMetricsBase
    {
        public VSProjectLocMetrics(VSProjectWithFileInfo projectWithFileInfo)
            : base(projectWithFileInfo.ProjectFileNameFull.ToString())
        {
            this.projectWithFileInfo = projectWithFileInfo;
        }

        public VSProjectWithFileInfo ProjectWithFileInfo
        {
            get { return projectWithFileInfo; }
        }

        /// <summary>
        /// Calculates the loc metrics for the whole project.
        /// </summary>
        /// <param name="projectWithFileInfo">The project info.</param>
        /// <param name="map">The map of <see cref="ILocStats"/> objects which can calculate LoC metrics for different source file types.</param>
        /// <returns>
        /// Returns the VSProjectLocMetrics instance.
        /// </returns>
        public static VSProjectLocMetrics CalculateLocForProject(VSProjectWithFileInfo projectWithFileInfo, LocStatsMap map)
        {
            VSProjectLocMetrics projectMetrics = new VSProjectLocMetrics(projectWithFileInfo);

            //For each Item file in project
            foreach (VSProjectItem item in projectWithFileInfo.Project.Items)
            {
                if (item.ItemType == VSProjectItem.CompileItem ||
                    item.ItemType == VSProjectItem.Content)
                {
                    string filePath = projectMetrics.ProjectWithFileInfo.ProjectDirectoryPath.AddFileName(item.Item)
                        .ToString();
                    SourceFileLocMetrics sourceFile = SourceFileLocMetrics.CalcLocStatData(filePath, map);

                    // make sure the file was not ignored (it wasn't a source file)
                    if (sourceFile != null)
                        projectMetrics.AddLocMetrics(sourceFile);
                }
            }
            
            return projectMetrics;
        }

        private VSProjectWithFileInfo projectWithFileInfo;
    }
}
