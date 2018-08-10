using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Shell;
using KoiCatalog.App;

namespace KoiCatalog.Gui.ValueConverters
{
    [ValueConversion(typeof(DatabaseLoadingState), typeof(TaskbarItemProgressState))]
    sealed class TaskbarItemProgressStateValueConverter : IValueConverter
    {
        public static TaskbarItemProgressStateValueConverter Instance { get; } =
            new TaskbarItemProgressStateValueConverter();

        private TaskbarItemProgressStateValueConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value as DatabaseLoadingState?;
            switch (status)
            {
                case DatabaseLoadingState.None:
                    return TaskbarItemProgressState.None;
                case DatabaseLoadingState.Normal:
                    return TaskbarItemProgressState.Normal;
                case DatabaseLoadingState.Indeterminate:
                    return TaskbarItemProgressState.Indeterminate;
                default:
                    return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
