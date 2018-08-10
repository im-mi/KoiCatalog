using System;
using System.Linq;

namespace KoiCatalog.Data
{
    public sealed class FilteredEntity : IReadOnlyEntity
    {
        private IReadOnlyEntity Entity { get; }
        private IReadOnlyComponentFilter ComponentFilter { get; }

        public FilteredEntity(IReadOnlyEntity entity, IReadOnlyComponentFilter componentFilter)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (componentFilter == null) throw new ArgumentNullException(nameof(componentFilter));
            Entity = entity;
            ComponentFilter = componentFilter;
        }

        public T GetComponent<T>(ComponentTypeCode<T> typeCode) where T : Component, new() =>
            ComponentFilter[typeCode] ? Entity.GetComponent(typeCode) : null;

        public Component GetComponent(ComponentTypeCode typeCode) =>
            ComponentFilter[typeCode] ? Entity.GetComponent(typeCode) : null;

        public Component[] GetComponents() =>
            Entity.GetComponents().Where(i => ComponentFilter[ComponentTypeCode.Get(i.GetType())]).ToArray();
    }
}
