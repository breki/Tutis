using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FinanceReport.DataModel
{
    public class TableRow
    {
        public TableRow(int id)
        {
            this.id = id;
        }

        public int Id
        {
            get { return id; }
        }

        public IDictionary<string, object> Values
        {
            get { return values; }
        }

        public void AddValue (string key, object value)
        {
            values.Add(key, value);
        }

        public override string ToString ()
        {
            StringBuilder s = new StringBuilder();
            s.AppendFormat(CultureInfo.InvariantCulture, "{0} (", id);

            foreach (KeyValuePair<string, object> pair in values)
                s.AppendFormat(CultureInfo.InvariantCulture, "{0}={1}, ", pair.Key, pair.Value);

            s.Append(")");

            return s.ToString ();
        }

        private int id;
        private Dictionary<string, object> values = new Dictionary<string, object>();
    }
}