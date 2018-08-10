using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace KoiCatalog.Gui.ValueConverters
{
    sealed class ColorValueConverter : IValueConverter
    {
        public static ColorValueConverter Instance { get; } = new ColorValueConverter();

        private ColorValueConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case Color _:
                    return value;
                case PalettePal.Color color:
                    return Color.FromArgb(color.A, color.R, color.G, color.B);
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
