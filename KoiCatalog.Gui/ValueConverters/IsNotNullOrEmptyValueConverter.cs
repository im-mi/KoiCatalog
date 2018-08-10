using System;
using System.Globalization;
using System.Windows.Data;

namespace KoiCatalog.Gui.ValueConverters
{
    sealed class IsNotNullOrEmptyValueConverter : IValueConverter
    {
        public static IsNotNullOrEmptyValueConverter Instance { get; } = new IsNotNullOrEmptyValueConverter();

        private IsNotNullOrEmptyValueConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is string str)
                    return str != string.Empty;
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
