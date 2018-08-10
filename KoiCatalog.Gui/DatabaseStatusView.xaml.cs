using System.Windows;
using System.Windows.Data;
using KoiCatalog.App;
using KoiCatalog.Data;

namespace KoiCatalog.Gui
{
    public partial class DatabaseStatusView
    {
        public DatabaseStatus DatabaseStatus
        {
            get => (DatabaseStatus)GetValue(DatabaseStatusProperty);
            set => SetValue(DatabaseStatusProperty, value);
        }
        public static readonly DependencyProperty DatabaseStatusProperty =
            DependencyProperty.Register(nameof(DatabaseStatus), typeof(DatabaseStatus), typeof(DatabaseStatusView));

        public DatabaseStatusView()
        {
            InitializeComponent();
        }

        private void EventLogErrors_OnFilter(object sender, FilterEventArgs e)
        {
            var ev = (DatabaseEvent)e.Item;
            e.Accepted = ev.Severity == EventSeverity.Error;
        }
    }
}
