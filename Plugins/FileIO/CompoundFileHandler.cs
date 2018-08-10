using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KoiCatalog.Plugins.FileIO
{
    public sealed class CompoundFileHandler : FileHandler
    {
        public CompoundFileHandler(Type[] handlerTypes)
        {
            if (handlerTypes == null) throw new ArgumentNullException(nameof(handlerTypes));
            Handlers = ResolveDependencies(handlerTypes)
                .Select(Activator.CreateInstance)
                .Cast<FileHandler>()
                .ToList()
                .AsReadOnly();
        }

        private static Type[] ResolveDependencies(Type[] types)
        {
            // Todo: Support nested dependencies.
            // Todo: Detect circular dependencies.
            var resolvedTypes = new List<Type>();
            foreach (var type in types)
            {
                var dependencies = type.GetCustomAttributes<FileHandlerDependencyAttribute>()
                    .Select(i => i.DependencyType);
                resolvedTypes.AddRange(dependencies);
                resolvedTypes.Add(type);
            }
            return resolvedTypes.Distinct().ToArray();
        }

        public override void HandleFile(FileLoader loader)
        {
            foreach (var fileHandler in Handlers)
            {
                try
                {
                    fileHandler.HandleFile(loader);
                }
                catch (Exception ex)
                {
                    loader.LogError(ex.Message);
                }
            }
        }

        public override void FinishHandleFile(FileLoader loader)
        {
            foreach (var fileHandler in Handlers.Reverse())
            {
                try
                {
                    fileHandler.FinishHandleFile(loader);
                }
                catch (Exception ex)
                {
                    loader.LogError(ex.Message);
                }
            }
        }

        private IEnumerable<FileHandler> Handlers { get; }
    }
}
