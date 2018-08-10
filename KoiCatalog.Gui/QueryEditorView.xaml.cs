using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using KoiCatalog.App;

namespace KoiCatalog.Gui
{
    public partial class QueryEditorView
    {
        public QueryEditor QueryEditor
        {
            get => (QueryEditor)GetValue(QueryEditorProperty);
            set => SetValue(QueryEditorProperty, value);
        }
        public static readonly DependencyProperty QueryEditorProperty =
            DependencyProperty.Register(nameof(QueryEditor), typeof(QueryEditor), typeof(QueryEditorView));

        public QueryEditorView()
        {
            InitializeComponent();
        }
        
        private void SelectAll_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var parameter = (sender as FrameworkElement)?.DataContext as QueryEditorParameter;
            parameter?.SelectableValues.ForEach(i => i.IsSelected = true);
        }

        private void SelectNone_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var parameter = (sender as FrameworkElement)?.DataContext as QueryEditorParameter;
            parameter?.SelectableValues.ForEach(i => i.IsSelected = false);
        }

        private void SelectInverse_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var parameter = (sender as FrameworkElement)?.DataContext as QueryEditorParameter;
            parameter?.SelectableValues.ForEach(i => i.IsSelected = !i.IsSelected);
        }
        
        private void UIElement_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            ((UIElement)sender).Focus();
        }
    }
}
