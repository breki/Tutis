using System.Collections.Generic;
using System.Xml;

namespace LineByLine.Console.Metrics
{
    /// <summary>
    /// Abstract class derived from LocMetricsBase. 
    /// Used to hold groups (nodes) of loc metrics items
    /// </summary>
    public abstract class GroupLocMetricsBase : LocMetricsBase
    {
        public IList<LocMetricsBase> GroupLocStatsData
        {
            get { return groupLocStatsData; }
        }

        /// <summary>
        /// Gets the loc stats data.
        /// </summary>
        /// <returns>Returns the combined loc stats of the group</returns>
        public override LocStatsData GetLocStatsData()
        {
            LocStatsData groupData = new LocStatsData(0, 0, 0);

            foreach (LocMetricsBase childLocMetrics in groupLocStatsData)
            {
                LocStatsData childLocStatsData = childLocMetrics.GetLocStatsData();
                groupData.Add(childLocStatsData);
            }

            return groupData;
        }

        protected GroupLocMetricsBase(string fileName)
            : base(fileName)
        {
        }

        /// <summary>
        /// Adds another loc metrics item to teh group.
        /// </summary>
        /// <param name="locMetrics">Loc metrics item.</param>
        protected void AddLocMetrics(LocMetricsBase locMetrics)
        {
            groupLocStatsData.Add(locMetrics);
        }

        /// <summary>
        /// Creates XML for all child items.
        /// </summary>
        /// <param name="writer">The parent (current) XML writer.</param>
        protected override void WriteSubitemsXml(XmlWriter writer)
        {
            writer.WriteStartElement("Subitem");

            foreach (LocMetricsBase childLocMetrics in groupLocStatsData)
            {
                childLocMetrics.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// The list of loc metrics items.
        /// </summary>
        private readonly List<LocMetricsBase> groupLocStatsData = new List<LocMetricsBase>();
    }
}
