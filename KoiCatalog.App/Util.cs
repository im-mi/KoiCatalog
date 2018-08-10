using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KoiCatalog.App
{
    public static class Util
    {
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (action == null) throw new ArgumentNullException(nameof(action));
            foreach (var item in self)
                action.Invoke(item);
        }

        public static void AddRange<T>(this Collection<T> self, IEnumerable<T> collection)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            collection.ForEach(self.Add);
        }

        private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(byte),
            typeof(sbyte),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
        };

        private static readonly HashSet<Type> NumericFractionalTypes = new HashSet<Type>
        {
            typeof(float),
            typeof(double),
            typeof(decimal),
        };

        public static bool IsNumericType(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return NumericTypes.Contains(type) ||
                   NumericTypes.Contains(Nullable.GetUnderlyingType(type));
        }
        
        public static bool IsNumericFractionalType(this Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return NumericFractionalTypes.Contains(type) ||
                   NumericFractionalTypes.Contains(Nullable.GetUnderlyingType(type));
        }
    }
}
