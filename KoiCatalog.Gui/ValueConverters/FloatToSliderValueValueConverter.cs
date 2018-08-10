using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using KoiCatalog.App;

namespace KoiCatalog.Gui.ValueConverters
{
    sealed class FloatToSliderValueValueConverter : IValueConverter
    {
        public static FloatToSliderValueValueConverter Instance { get; } = new FloatToSliderValueValueConverter();

        private FloatToSliderValueValueConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            if (!value.GetType().IsNumericType()) return DependencyProperty.UnsetValue;
            // Decimal is used for maximum precision, otherwise there can be rounding errors that take it out of sync with the in-game values.
            var numericValue = Math.Truncate(System.Convert.ToDecimal(value) * 100);
            return string.Format(culture, "{0:n0}", numericValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
