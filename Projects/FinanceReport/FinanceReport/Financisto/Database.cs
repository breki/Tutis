using System.Collections.Generic;

namespace FinanceReport.Financisto
{
    public class Database
    {
        public IDictionary<string, DatabaseTable> Tables
        {
            get { return tables; }
        }

        public DatabaseTable GetTable(string tableName)
        {
            if (!tables.ContainsKey(tableName))
                tables.Add(tableName, new DatabaseTable(tableName));

            return tables[tableName];
        }

        private Dictionary<string, DatabaseTable> tables = new Dictionary<string, DatabaseTable>();
    }
}