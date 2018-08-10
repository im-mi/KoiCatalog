using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using KoiCatalog.Plugins.Koikatu;

namespace KoiCatalog.Gui.ValueConverters
{
    [ValueConversion(typeof(Sex), typeof(string))]
    sealed class SexToStringValueConverter : IValueConverter
    {
        public static SexToStringValueConverter Instance { get; } = new SexToStringValueConverter();

        private SexToStringValueConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value as Sex?)
            {
                case Sex.Male:
                    return "♂";
                case Sex.Female:
                    return "♀";
                case null:
                    return DependencyProperty.UnsetValue;
                default:
                {
                    try
                    {
                        return System.Convert.ChangeType(value, typeof(int));
                    }
                    catch (Exception)
                    {
                        return DependencyProperty.UnsetValue;
                    }
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
