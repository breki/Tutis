using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace FinanceReport.DataModel
{
    public class FinancistoReader
    {
        public Database ReadDatabaseFromFile(string fileName)
        {
            using (Stream stream = File.OpenRead(fileName))
                return ReadDatabaseFromStream(stream);
        }

        public Database ReadDatabaseFromZipile (string fileName)
        {
            using (Stream stream = File.OpenRead (fileName))
            using (GZipStream gzstream = new GZipStream (stream, CompressionMode.Decompress))
            using (MemoryStream memStream = new MemoryStream ())
            {
                byte[] buffer = new byte[10000];

                while (true)
                {
                    int actuallyRead = gzstream.Read (buffer, 0, buffer.Length);
                    if (actuallyRead == 0)
                        break;

                    memStream.Write (buffer, 0, actuallyRead);
                }

                memStream.Seek (0, SeekOrigin.Begin);
                return ReadDatabaseFromStream (memStream);
            }
        }

        public Database ReadDatabaseFromStream(Stream stream)
        {
            using (reader = new StreamReader (stream))
            {
                db = new Database();

                ReadHeader();
                ReadData();

                return db;
            }
        }

        private void ReadHeader()
        {
            do
            {
                ReadNextLine();
            } 
            while (line != "#START");
        }

        private void ReadData()
        {
            while (ReadNextTableRow ())
            {
            }
        }

        private bool ReadNextTableRow()
        {
            ReadNextLine();
            if (line == "#END")
                return false;

            ReadRowData();
            return true;
        }

        private void ReadRowData()
        {
            TableRow row = null;
            DatabaseTable table = null;

            while(true)
            {
                KeyValuePair<string, string> entry = ReadRowColumn ();

                if (entry.Key == null)
                    break;

                if (entry.Key == "$ENTITY" && table == null)
                    table = db.GetTable(entry.Value);
                else if (entry.Key == "_id")
                    row = table.CreateRow(int.Parse(entry.Value, CultureInfo.InvariantCulture));
                else
                {
                    if (row != null)
                        row.AddValue (entry.Key, entry.Value);
                }

                ReadNextLine ();
            }
        }

        private KeyValuePair<string, string> ReadRowColumn()
        {
            if (line.StartsWith ("$$"))
                return new KeyValuePair<string, string>();

            int separatorIndex = line.IndexOf(':');
            string key = line.Substring(0, separatorIndex);
            string value = line.Substring(separatorIndex + 1);

            return new KeyValuePair<string, string>(key, value);
        }

        private void ReadNextLine()
        {
            line = reader.ReadLine();
        }

        private StreamReader reader;
        private Database db;
        private string line;
    }
}