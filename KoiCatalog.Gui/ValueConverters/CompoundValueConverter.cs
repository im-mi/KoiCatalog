using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace KoiCatalog.Gui.ValueConverters
{
    public sealed class CompoundValueConverter : IValueConverter, IEnumerable<IValueConverter>
    {
        public List<IValueConverter> Converters { get; } = new List<IValueConverter>();

        public void Add(IValueConverter converter)
        {
            Converters.Add(converter);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Converters.Aggregate(value, (current, converter) =>
                converter.Convert(current, targetType, parameter, culture));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Converters.Aggregate(value, (current, converter) =>
                converter.ConvertBack(current, targetType, parameter, culture));
        }

        public IEnumerator<IValueConverter> GetEnumerator() => Converters.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
