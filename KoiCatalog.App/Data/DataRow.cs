using System;

namespace KoiCatalog.App.Data
{
    public sealed class DataRow
    {
        private DataTable DataTable { get; }
        private object[] Values { get; }

        public object this[int index]
        {
            get => Values[index];
            set => Values[index] = value;
        }

        internal DataRow(DataTable dataTable)
        {
            if (dataTable == null) throw new ArgumentNullException(nameof(dataTable));
            DataTable = dataTable;
            Values = new object[dataTable.Columns.Count];
        }
    }
}
