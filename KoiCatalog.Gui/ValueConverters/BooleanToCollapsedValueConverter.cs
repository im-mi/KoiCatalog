using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KoiCatalog.Gui.ValueConverters
{
    [ValueConversion(typeof(Visibility), typeof(bool))]
    sealed class BooleanToCollapsedValueConverter : IValueConverter
    {
        public static BooleanToCollapsedValueConverter Instance { get; } = new BooleanToCollapsedValueConverter();

        private BooleanToCollapsedValueConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = value as Visibility?;
            if (visibility == null) return DependencyProperty.UnsetValue;
            return visibility != Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = value as bool?;
            if (result == null) return DependencyProperty.UnsetValue;
            return result == true ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
