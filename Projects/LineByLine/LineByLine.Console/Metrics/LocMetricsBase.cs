using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Xml;

namespace LineByLine.Console.Metrics
{
    /// <summary>
    /// Abstract class for holding loc metrics objects.
    /// </summary>
    public abstract class LocMetricsBase
    {
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName
        {
            get { return fileName; }
        }

        public string FileType
        {
            get { return fileType; }
        }

        protected LocMetricsBase(string fileName)
        {
            this.fileName = fileName;
            this.fileType = Path.GetExtension(fileName);
        }

        /// <summary>
        /// Gets the loc stats data.
        /// </summary>
        /// <returns>Returns loc metrics.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public abstract LocStatsData GetLocStatsData();

        /// <summary>
        /// Writes the XML for current file.
        /// </summary>
        /// <param name="writer">The XML writer.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Item");
            writer.WriteAttributeString("FileName", this.FileName);
            writer.WriteAttributeString("FileType", this.FileType);

            LocStatsData locStatsData = GetLocStatsData();

            writer.WriteStartElement("LoC");
            writer.WriteElementString("ELoC", Convert.ToString(locStatsData.Eloc, CultureInfo.InvariantCulture));
            writer.WriteElementString("SLoC", Convert.ToString(locStatsData.Sloc, CultureInfo.InvariantCulture));
            writer.WriteElementString("CLoc", Convert.ToString(locStatsData.Cloc, CultureInfo.InvariantCulture));
            writer.WriteEndElement();

            WriteSubitemsXml(writer);

            writer.WriteEndElement();
        }

        protected abstract void WriteSubitemsXml(XmlWriter writer);

        private string fileName;
        private string fileType;
    }
}
