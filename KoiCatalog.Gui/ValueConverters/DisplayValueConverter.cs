using System;
using System.Globalization;
using System.Windows.Data;
using KoiCatalog.App;
using KoiCatalog.Util;

namespace KoiCatalog.Gui.ValueConverters
{
    [ValueConversion(typeof(object), typeof(string))]
    sealed class DisplayValueConverter : IValueConverter
    {
        public static DisplayValueConverter Instance { get; } = new DisplayValueConverter();

        private DisplayValueConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var result = ConvertToDisplayString(value, culture);
            if (value.GetType().IsEnum)
                result = StringUtil.FormatCamelcase(result);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        
        private static string ConvertToDisplayString(object value, CultureInfo culture)
        {
            if (value == null) return null;
            if (value.GetType().IsNumericFractionalType())
                return string.Format(culture, "{0:n}", value);
            if (value.GetType().IsNumericType())
                return string.Format(culture, "{0:n0}", value);
            if (value is IConvertible)
                return (string)System.Convert.ChangeType(value, typeof(string), culture);
            return value.ToString();
        }
    }
}
