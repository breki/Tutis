using System.Collections.Generic;

namespace FinanceReport.Financisto
{
    public class DatabaseTable
    {
        public DatabaseTable(string tableName)
        {
            this.tableName = tableName;
        }

        public IEnumerable<TableRow> Rows
        {
            get { return rows.Values; }
        }

        public TableRow CreateRow(int rowId)
        {
            TableRow row = new TableRow(rowId);
            rows.Add(rowId, row);
            return row;
        }

        private readonly string tableName;
        private SortedList<int, TableRow> rows = new SortedList<int, TableRow>();
    }
}