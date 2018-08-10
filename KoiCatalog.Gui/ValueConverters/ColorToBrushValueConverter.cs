using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace KoiCatalog.Gui.ValueConverters
{
    sealed class ColorToBrushValueConverter : IValueConverter
    {
        public static ColorToBrushValueConverter Instance { get; } = new ColorToBrushValueConverter();

        private ColorToBrushValueConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = ColorValueConverter.Instance.Convert(value, typeof(Color), parameter, culture) as Color?;
            if (color == null)
                return DependencyProperty.UnsetValue;
            return new SolidColorBrush(color.Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
