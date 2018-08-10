using System;

namespace KoiCatalog.Data
{
    [Serializable]
    public abstract class Component
    {
        internal static void CheckComponentType(Type componentType)
        {
            if (componentType == null) throw new ArgumentNullException(nameof(componentType));
            if (!typeof(Component).IsAssignableFrom(componentType))
                throw new ArgumentException($"Type must be assignable from {typeof(Component)}.", nameof(componentType));
            if (componentType.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException("Type must have a public parameterless constructor.", nameof(componentType));
        }
    }

    [Serializable]
    public abstract class Component<T> : Component where T : Component, new()
    {
        public static ComponentTypeCode<T> TypeCode { get; } = ComponentTypeCode.Get<T>();
    }
}
