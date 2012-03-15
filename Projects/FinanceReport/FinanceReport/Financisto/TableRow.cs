using System.Collections.Generic;

namespace FinanceReport.Financisto
{
    public class TableRow
    {
        public TableRow(int id)
        {
            this.id = id;
        }

        public IDictionary<string, object> Values
        {
            get { return values; }
        }

        public void AddValue (string key, object value)
        {
            values.Add(key, value);
        }

        private int id;
        private Dictionary<string, object> values = new Dictionary<string, object>();
    }
}