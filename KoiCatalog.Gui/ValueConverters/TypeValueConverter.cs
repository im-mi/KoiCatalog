using System;
using System.Globalization;
using System.Windows.Data;

namespace KoiCatalog.Gui.ValueConverters
{
    [ValueConversion(typeof(object), typeof(Type))]
    sealed class TypeValueConverter : IValueConverter
    {
        public static TypeValueConverter Instance { get; } = new TypeValueConverter();

        private TypeValueConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.GetType();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
