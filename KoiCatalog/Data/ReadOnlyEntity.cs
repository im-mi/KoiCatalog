using System;

namespace KoiCatalog.Data
{
    public sealed class ReadOnlyEntity : IReadOnlyEntity
    {
        private IReadOnlyEntity Source { get; }

        public ReadOnlyEntity(IReadOnlyEntity source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            Source = source;
        }

        public T GetComponent<T>(ComponentTypeCode<T> typeCode) where T : Component, new() =>
            Source.GetComponent(typeCode);

        public Component GetComponent(ComponentTypeCode typeCode) =>
            Source.GetComponent(typeCode);

        public Component[] GetComponents() =>
            Source.GetComponents();
    }
}
