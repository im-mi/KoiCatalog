using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using KoiCatalog.App.Data;
using KoiCatalog.Data;
using KoiCatalog.Gui.ValueConverters;
using KoiCatalog.Plugins.Koikatu;
using KoiCatalog.Util;
using PalettePal;
using Visibility = System.Windows.Visibility;

namespace KoiCatalog.Gui
{
    public partial class DataTableView : INotifyPropertyChanged
    {
        public DataTableView()
        {
            InitializeComponent();
        }

        public DataTable DataTable
        {
            get => (DataTable)GetValue(DataTableProperty);
            set => SetValue(DataTableProperty, value);
        }
        public static readonly DependencyProperty DataTableProperty = DependencyProperty.Register(
            nameof(DataTable), typeof(DataTable), typeof(DataTableView),
            new PropertyMetadata(DataTableChanged));

        private static void DataTableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DataTableView)d).DataTableChanged((DataTable)e.OldValue, (DataTable)e.NewValue);
        }

        private void DataTableChanged(DataTable oldValue, DataTable newValue)
        {
            if (oldValue != null)
            {
                oldValue.Columns.CollectionChanged -= ColumnsOnCollectionChanged;
            }

            if (newValue != null)
            {
                newValue.Columns.CollectionChanged += ColumnsOnCollectionChanged;
            }

            RefreshColumns();
        }

        private void ColumnsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshColumns();
        }

        private void RefreshColumns()
        {
            // Todo: Only add or remove columns that actually changed.
            DataGrid.Columns.Clear();
            if (DataTable == null) return;

            var columnInfos = DataTable.Columns
                .Select((item, columnIndex) => new
                {
                    ColumnIndex = columnIndex,
                    item.Property,
                    Display = item.Property.GetCustomAttribute<DisplayAttribute>() ?? DisplayAttribute.Default,
                })
                .Where(i => i.Display.Visibility != KoiCatalog.Data.Visibility.Never)
                .OrderBy(i => i.Display.Order);

            foreach (var columnInfo in columnInfos)
            {
                DataGridColumn column;

                var propertyPath = new PropertyPath($"[{columnInfo.ColumnIndex}]");

                if (columnInfo.Property.PropertyType == typeof(Uri))
                {
                    var templateColumn = new DataGridTemplateColumn();
                    column = templateColumn;
                    var binding = new Binding { Path = propertyPath };
                    // Todo: Proper async image loading. Using an async binding merely adds a delay.
                    templateColumn.CellTemplate = CreateContentHostDataTemplate(UriDataTemplate, new Binding { Path = propertyPath, IsAsync = true });
                    column.ClipboardContentBinding = binding;
                    templateColumn.HeaderStyle = CenteredColumnHeaderStyle;
                }
                else if (columnInfo.Property.PropertyType == typeof(Color))
                {
                    var templateColumn = new DataGridTemplateColumn();
                    column = templateColumn;
                    var binding = new Binding { Path = propertyPath };
                    templateColumn.CellTemplate = CreateContentHostDataTemplate(ColorDataTemplate, binding);
                    column.ClipboardContentBinding = binding;
                    templateColumn.SortMemberPath = propertyPath.Path;
                }
                else if (columnInfo.Property.PropertyType == typeof(Sex))
                {
                    var templateColumn = new DataGridTemplateColumn();
                    column = templateColumn;
                    var binding = new Binding { Path = propertyPath };
                    templateColumn.CellTemplate = CreateContentHostDataTemplate(SexDataTemplate, binding);
                    column.ClipboardContentBinding = new Binding
                    {
                        Path = propertyPath,
                        Converter = DisplayValueConverter.Instance,
                    };
                    templateColumn.HeaderStyle = CenteredColumnHeaderStyle;
                    templateColumn.SortMemberPath = propertyPath.Path;
                }
                else if (typeof(IEnumerable<Color>).IsAssignableFrom(columnInfo.Property.PropertyType))
                {
                    var templateColumn = new DataGridTemplateColumn();
                    column = templateColumn;
                    var binding = new Binding { Path = propertyPath };
                    templateColumn.CellTemplate = CreateContentHostDataTemplate(ColorCollectionDataTemplate, binding);
                    column.ClipboardContentBinding = new Binding { Path = propertyPath, Converter = StringJoinValueConverter };
                    templateColumn.SortMemberPath = $"{propertyPath.Path}[0]";
                }
                else
                {
                    var textColumn = new DataGridTextColumn();
                    column = textColumn;
                    // Todo: Avoid setting set ElementStyle directly. Set a default style instead.
                    textColumn.ElementStyle = DataGridTextColumnElementStyle;
                    var binding = new Binding
                    {
                        Path = propertyPath,
                        Converter = DisplayValueConverter.Instance
                    };
                    textColumn.Binding = binding;

                    // Todo: Don't assume all floats are slider values.
                    if (columnInfo.Property.PropertyType == typeof(float))
                    {
                        binding.Converter = FloatToSliderValueValueConverter.Instance;
                        column.ClipboardContentBinding = new Binding { Path = propertyPath };
                    }
                }

                if (columnInfo.Display.Visibility == KoiCatalog.Data.Visibility.Hidden)
                    column.Visibility = Visibility.Collapsed;
                column.Header = columnInfo.Display.Name ?? StringUtil.FormatCamelcase(columnInfo.Property.Name);
                column.Width = columnInfo.Display.Width;
                column.MinWidth = columnInfo.Display.MinWidth;

                DataGrid.Columns.Add(column);
            }
        }

        private static DataTemplate CreateContentHostDataTemplate(DataTemplate dataTemplate, Binding binding)
        {
            var contentControl = new FrameworkElementFactory(typeof(ContentControl));
            contentControl.SetValue(ContentTemplateProperty, dataTemplate);
            contentControl.SetValue(ContentProperty, binding);
            contentControl.SetValue(FocusableProperty, false);
            var dataTemplate2 = new DataTemplate { VisualTree = contentControl };
            return dataTemplate2;
        }

        private DataTemplate UriDataTemplate => (DataTemplate)DataGrid.FindResource("UriDataTemplate");
        private DataTemplate ColorDataTemplate => (DataTemplate)DataGrid.FindResource("ColorDataTemplate");
        private DataTemplate SexDataTemplate => (DataTemplate)DataGrid.FindResource("SexDataTemplate");
        private DataTemplate ColorCollectionDataTemplate => (DataTemplate)DataGrid.FindResource("ColorCollectionDataTemplate");
        private Style DataGridTextColumnElementStyle => (Style)DataGrid.FindResource("DataGridTextColumnElementStyle");
        private Style CenteredColumnHeaderStyle => (Style)DataGrid.FindResource("CenteredColumnHeaderStyle");

        private StringJoinValueConverter StringJoinValueConverter { get; } =
            new StringJoinValueConverter { Separator = " " };

        public int VisibleDataGridColumnsCount =>
            DataGrid.Columns.Count(i => i.Visibility == Visibility.Visible);

        private Point dragStart;

        private void Uri_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            var element = (UIElement)sender;
            dragStart = e.GetPosition(null);
            element.CaptureMouse();
        }

        private void Uri_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            var element = (FrameworkElement)sender;
            if (!ReferenceEquals(Mouse.Captured, element)) return;

            var mousePosition = e.GetPosition(null);
            var mouseDelta = dragStart - mousePosition;

            if (Math.Abs(mouseDelta.X) <= SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(mouseDelta.Y) <= SystemParameters.MinimumVerticalDragDistance)
            {
                return;
            }

            var uri = element.DataContext as Uri;
            if (uri == null) return;
            if (!uri.IsAbsoluteUri) return;
            var dataObject = new DataObject(DataFormats.FileDrop, new[] { Uri.UnescapeDataString(uri.AbsolutePath) });
            DragDrop.DoDragDrop(element, dataObject, DragDropEffects.Copy);
        }

        private void Uri_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Released) return;
            var element = (UIElement)sender;
            element.ReleaseMouseCapture();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ContextMenu_OnOpened(object sender, RoutedEventArgs e)
        {
            // Todo: Figure out a better way to indicate the number of visible columns to the view.
            OnPropertyChanged(nameof(VisibleDataGridColumnsCount));
        }

        private void ResetSortOrder_OnClick(object sender, RoutedEventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(DataGrid.ItemsSource);
            if (view == null) return;
            view.SortDescriptions.Clear();
            foreach (var column in DataGrid.Columns)
                column.SortDirection = null;
        }
    }
}
