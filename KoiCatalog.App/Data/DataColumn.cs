using System;
using System.Reflection;

namespace KoiCatalog.App.Data
{
    public sealed class DataColumn
    {
        public PropertyInfo Property { get; }

        public DataColumn(PropertyInfo property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            Property = property;
        }
    }
}
