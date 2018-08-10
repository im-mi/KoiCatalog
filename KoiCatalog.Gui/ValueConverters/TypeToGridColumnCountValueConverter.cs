using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using KoiCatalog.Plugins.Koikatu;

namespace KoiCatalog.Gui.ValueConverters
{
    sealed class TypeToGridColumnCountValueConverter : IValueConverter
    {
        public static TypeToGridColumnCountValueConverter Instance { get; } =
            new TypeToGridColumnCountValueConverter();

        private TypeToGridColumnCountValueConverter() { }

        private static readonly Dictionary<Type, int> TypeToColumns = new Dictionary<Type, int>
        {
            { typeof(HeightType), 3 },
            { typeof(BustSizeType), 3 },
            { typeof(BloodType), 4 },
            { typeof(HairStyle), 3 },
            { typeof(ClubActivity), 1 },
            { typeof(SkinType), 3 },
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value as Type;
            if (type == null)
                return DependencyProperty.UnsetValue;
            if (TypeToColumns.TryGetValue(type, out var columns))
                return columns;
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
