using System.IO;
using System.Xml;

namespace LineByLine.Console.Metrics
{
    /// <summary>
    /// Implementation of LocMetricsBase. Represents a single source file item
    /// and its loc metrics.
    /// </summary>
    public class SourceFileLocMetrics : LocMetricsBase
    {
        public SourceFileLocMetrics(string fileName) : base(fileName)
        {
        }

        /// <summary>
        /// Calculates the loc stat data of the file.
        /// </summary>
        /// <param name="sourceFileName">The source file to analyze.</param>
        /// <param name="map">The map of <see cref="ILocStats"/> objects which can calculate LoC metrics for different source file types.</param>
        /// <returns>A newly created <see cref="SourceFileLocMetrics"/> object containing LoC data for the source file; 
        /// <c>null</c> if the file was ignored by the <see cref="map"/></returns>
        public static SourceFileLocMetrics CalcLocStatData(string sourceFileName, LocStatsMap map)
        {
            SourceFileLocMetrics metrics = new SourceFileLocMetrics(sourceFileName);

            using (Stream streamOfFile = File.OpenRead(sourceFileName))
            {
                string fileExtension = Path.GetExtension(sourceFileName);

                ILocStats locStats = map.GetLocStatsForExtension(fileExtension);
                if (locStats != null)
                {
                    metrics.locStatsData = locStats.CountLocStream(streamOfFile);
                    return metrics;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the loc stats data.
        /// </summary>
        /// <returns>Returns the loc stat metrics of the file.</returns>
        public override LocStatsData GetLocStatsData()
        {
            return locStatsData;
        }

        protected override void WriteSubitemsXml(XmlWriter writer)
        {
            // no subitems here
        }

        private LocStatsData locStatsData = new LocStatsData();
    }
}
