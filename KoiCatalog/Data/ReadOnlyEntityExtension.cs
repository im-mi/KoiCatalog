using System;

namespace KoiCatalog.Data
{
    public static class ReadOnlyEntityExtension
    {
        public static IReadOnlyEntity AsReadOnly(this IReadOnlyEntity self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            return new ReadOnlyEntity(self);
        }

        public static IReadOnlyEntity AsReadOnly(this IReadOnlyEntity self, IReadOnlyComponentFilter componentFilter)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (componentFilter == null) throw new ArgumentNullException(nameof(componentFilter));
            return new FilteredEntity(self, componentFilter);
        }
    }
}
