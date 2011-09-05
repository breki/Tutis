using System;
using System.Xml;
using LineByLine.Console.VSSolutionBrowsing;

namespace LineByLine.Console.Metrics
{
    /// <summary>
    /// Implementation of the GroupLocMetricsBase abstarct class.
    /// Represents a Visual studio solution with a list of
    /// VS projects.
    /// </summary>
    public class VSSolutionLocMetrics : GroupLocMetricsBase
    {
        public VSSolutionLocMetrics(string fileName) : base(fileName)
        {
        }

        public LocStatsMap LocStatsMap 
        { 
            get { return locStatsMap; } 
        }

        /// <summary>
        /// Calculates the loc for the whole solution.
        /// </summary>
        /// <param name="solutionFileName">Name of the solution file.</param>
        public void CalculateLocForSolution(string solutionFileName)
        {
            //VSSolutionLocMetrics metrics = new VSSolutionLocMetrics();

            //Load the solution, appropriate projects and their compile items.
            VSSolution solution = VSSolution.Load(solutionFileName);
            solution.LoadProjects();

            foreach (VSProjectInfo projectInfo in solution.Projects)
            {
                //just C# projects
                if (projectInfo.ProjectTypeGuid != VSProjectType.CSharpProjectType.ProjectTypeGuid)
                    continue;

                //Calculate the metrics for each containing project.
                VSProjectLocMetrics projectMetrics = VSProjectLocMetrics.CalculateLocForProject(
                    (VSProjectWithFileInfo)projectInfo, 
                    locStatsMap);
                AddLocMetrics(projectMetrics);
            }

            locCalculated = true;
        }

        /// <summary>
        /// Generates the XML report of the entire solution.
        /// </summary>
        /// <param name="filePath">The path of .sln file.</param>
        public void GenerateXmlReport(string filePath)
        {
            if (false == locCalculated)
                throw new InvalidOperationException("LoC was not calculated.");

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.Encoding = System.Text.Encoding.UTF8;
            writerSettings.Indent = true;

            using (XmlWriter xmlWriter = XmlWriter.Create(filePath, writerSettings))
            {
                xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                xmlWriter.WriteStartElement("Root");

                this.WriteXml(xmlWriter);

                xmlWriter.WriteEndElement();
            }
        }

        private bool locCalculated;
        private LocStatsMap locStatsMap = new LocStatsMap();
    }
}
