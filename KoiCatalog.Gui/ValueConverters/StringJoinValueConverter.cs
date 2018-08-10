using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace KoiCatalog.Gui.ValueConverters
{
    sealed class StringJoinValueConverter : IValueConverter
    {
        public string Separator { get; set; } = ", ";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case null:
                    return string.Empty;
                case IEnumerable items:
                    return string.Join(Separator, items.Cast<object>().Select(i => i?.ToString()));
                default:
                    return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
