using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using KoiCatalog.Data;

namespace KoiCatalog.Gui
{
    public partial class EventViewer
    {
        public ICollection<DatabaseEvent> Events
        {
            get => (ICollection<DatabaseEvent>)GetValue(MyPropertyProperty);
            set => SetValue(MyPropertyProperty, value);
        }
        
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register(nameof(Events), typeof(ICollection<DatabaseEvent>), typeof(EventViewer));
        
        public EventViewer()
        {
            InitializeComponent();
        }

        private void EventLogCollectionView_OnFilter(object sender, FilterEventArgs e)
        {
            var ignoredEventTypes = new[]
            {
                DatabaseEventType.LoadProgress,
            }.ToList();

            var ev = (DatabaseEvent)e.Item;
            e.Accepted = ev.Severity == EventSeverity.Error || !ignoredEventTypes.Contains(ev.EventType);
        }

        private void EventLogClear_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Events?.Clear();
        }

        private void Delete_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var ev = e.Parameter as DatabaseEvent;
            if (ev == null) return;
            // Todo: Remove based on index rather than equality.
            Events?.Remove(ev);
        }

        private void HandleButtonEvent_OnClick(object sender, RoutedEventArgs e)
        {
            // Prevent the outer button from detecting the click as well.
            e.Handled = true;
        }
    }
}
