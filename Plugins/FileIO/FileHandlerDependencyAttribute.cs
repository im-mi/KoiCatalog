using System;

namespace KoiCatalog.Plugins.FileIO
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class FileHandlerDependencyAttribute : Attribute
    {
        public Type DependencyType { get; }

        public FileHandlerDependencyAttribute(Type dependencyType)
        {
            if (dependencyType == null) throw new ArgumentNullException(nameof(dependencyType));
            if (!typeof(FileHandler).IsAssignableFrom(dependencyType))
                throw new ArgumentException($"Type is not assignable to {nameof(FileHandler)}.", nameof(dependencyType));
            if (dependencyType.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException("Type does not have a public parameterless constructor.", nameof(dependencyType));
            DependencyType = dependencyType;
        }
    }
}
