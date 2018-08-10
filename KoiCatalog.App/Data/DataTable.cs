using System.Collections.ObjectModel;

namespace KoiCatalog.App.Data
{
    public sealed class DataTable
    {
        public ObservableCollection<DataColumn> Columns { get; } =
            new ObservableCollection<DataColumn>();

        public ObservableCollection<DataRow> Rows { get; } =
            new ObservableCollection<DataRow>();

        public void Clear()
        {
            Rows.Clear();
        }

        public DataRow NewRow()
        {
            return new DataRow(this);
        }
    }
}
