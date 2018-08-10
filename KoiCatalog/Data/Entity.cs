using System;
using System.Collections.Generic;
using System.Linq;
using KoiCatalog.Util;

namespace KoiCatalog.Data
{
    [Serializable]
    public sealed class Entity : IReadOnlyEntity
    {
        private AutoSizingList<Component> Components { get; } = new AutoSizingList<Component>();

        public T GetComponent<T>(ComponentTypeCode<T> typeCode) where T : Component, new() =>
            (T)Components[typeCode.TypeCode];

        public Component GetComponent(ComponentTypeCode typeCode) =>
            Components[typeCode.TypeCode];

        /// <summary>
        /// Adds a component of the specified type to this entity.
        /// If the entity already has a component of the specified type, then no action is taken.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeCode"></param>
        /// <returns>The component that was added or the existing component if present.</returns>
        public T AddComponent<T>(ComponentTypeCode<T> typeCode) where T : Component, new()
        {
            var component = GetComponent(typeCode);
            if (component == null)
            {
                component = new T();
                Components[typeCode.TypeCode] = component;
            }
            return component;
        }

        public Component[] GetComponents() =>
            Components.Where(i => i != null).ToArray();

        public bool HasComponents(IEnumerable<ComponentTypeCode> typeCodes)
        {
            if (typeCodes == null) throw new ArgumentNullException(nameof(typeCodes));
            return typeCodes.All(typeCode => GetComponent(typeCode) != null);
        }

        /// <summary>
        /// Reorders components to match the local component type codes.
        /// </summary>
        public void LocalizeTypeCodes()
        {
            var oldComponents = Components.Where(i => i != null).ToList();
            Components.Clear();
            foreach (var component in oldComponents)
            {
                var typeCode = ComponentTypeCode.Get(component.GetType());
                Components[typeCode.TypeCode] = component;
            }
        }
    }
}
