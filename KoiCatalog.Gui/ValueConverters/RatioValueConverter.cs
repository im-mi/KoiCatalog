using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KoiCatalog.Gui.ValueConverters
{
    sealed class RatioMultiValueConverter : IMultiValueConverter
    {
        public static RatioMultiValueConverter Instance { get; } = new RatioMultiValueConverter();

        private RatioMultiValueConverter() { }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var value0 = (double)System.Convert.ChangeType(values[0], typeof(double));
                var value1 = (double)System.Convert.ChangeType(values[1], typeof(double));
                return value0 / value1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return DependencyProperty.UnsetValue;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
