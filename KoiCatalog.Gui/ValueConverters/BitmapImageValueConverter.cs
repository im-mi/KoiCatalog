using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace KoiCatalog.Gui.ValueConverters
{
    /// <summary>
    /// Converts a <see cref="T:System.Uri" /> or <see cref="T:System.String" /> to a
    /// <see cref="T:System.Windows.Media.Imaging.BitmapImage" />, without locking the source file.
    /// </summary>
    [ValueConversion(typeof(Uri), typeof(BitmapImage))]
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    sealed class BitmapImageValueConverter : IValueConverter
    {
        public static BitmapImageValueConverter Instance { get; } = new BitmapImageValueConverter();

        private BitmapImageValueConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Uri source;
            if (value is string s)
                source = new Uri(s);
            else if (value is Uri)
                source = (Uri)value;
            else
                return DependencyProperty.UnsetValue;

            try
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = source;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
            catch
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
