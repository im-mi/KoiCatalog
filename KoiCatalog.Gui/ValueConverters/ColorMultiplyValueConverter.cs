using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace KoiCatalog.Gui.ValueConverters
{
    sealed class ColorMultiplyValueConverter : IValueConverter
    {
        public Color Color { get; set; } = Colors.White;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Color)) return DependencyProperty.UnsetValue;
            var color = (Color)value;
            return Color.FromArgb(
                MultiplyColorChannel(color.A, Color.A),
                MultiplyColorChannel(color.R, Color.R),
                MultiplyColorChannel(color.G, Color.G),
                MultiplyColorChannel(color.B, Color.B)
            );
        }

        private static byte MultiplyColorChannel(byte value1, byte value2)
        {
            return (byte)(value1 * ((float)value2 / 0xFF));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
