using System;
using System.Collections.Generic;
using KoiCatalog.Util;

namespace KoiCatalog.Data
{
    public sealed class ReadOnlyComponentFilter : IReadOnlyComponentFilter
    {
        private readonly AutoSizingList<bool> Filter = new AutoSizingList<bool>();

        public bool this[ComponentTypeCode typeCode] =>
            Filter[typeCode.TypeCode];

        public ReadOnlyComponentFilter(IEnumerable<ComponentTypeCode> filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            foreach (var component in filter)
            {
                Filter[component.TypeCode] = true;
            }
        }
    }
}
