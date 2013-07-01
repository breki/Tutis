using System.Collections.Generic;

namespace FinanceReport.DataModel
{
    public class DatabaseTable
    {
        public DatabaseTable(string tableName)
        {
            this.tableName = tableName;
        }

        public SortedList<int, TableRow> Rows
        {
            get { return rows; }
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