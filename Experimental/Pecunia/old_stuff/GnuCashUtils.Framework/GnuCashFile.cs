using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace GnuCashUtils.Framework
{
    public class GnuCashFile
    {
        /// <summary>
        /// Gets or sets the GnuCash fileName.
        /// </summary>
        /// <value>The GnuCash fileName.</value>
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public XmlDocument XmlDocument
        {
            get { return xmlDoc; }
            set { xmlDoc = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GnuCashFile"/> class by
        /// loading the specified GnuCash file.
        /// </summary>
        /// <param name="fileName">The GnuCash fileName.</param>
        public GnuCashFile (string fileName)
        {
            Load (fileName);
        }

        /// <summary>
        /// Initializes a new empty instance of the <see cref="GnuCashFile"/> class.
        /// </summary>
        public GnuCashFile () { }

        /// <summary>
        /// Loads the specified GnuCash file.
        /// </summary>
        /// <param name="fileName">The fileName.</param>
        public void Load (string fileName)
        {
            this.fileName = fileName;
            xmlDoc = new XmlDocument ();

            using (Stream stream = File.Open (fileName, FileMode.Open, FileAccess.Read, FileShare.None))
                xmlDoc.Load (stream);

            xmlDoc.PreserveWhitespace = false;
        }

        public void Save (string fileName)
        {
            using (Stream stream = File.Open (fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                XmlWriterSettings settings = new XmlWriterSettings ();
                Encoding encoding = new UTF8Encoding (false);
                settings.Encoding = encoding;
                settings.NewLineOnAttributes = false;
                settings.Indent = true;
                settings.NewLineChars = Environment.NewLine;
                using (XmlWriter writer = XmlWriter.Create (stream, settings))
                    xmlDoc.Save (writer);
            }
        }

        public void Save ()
        {
            Save (fileName);
        }

        private string fileName;
        private XmlDocument xmlDoc;
    }
}
