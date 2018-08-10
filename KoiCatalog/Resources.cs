using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

namespace KoiCatalog
{
    public static class Resources
    {
        private static string GetResourcesDirectory(Assembly assembly = null)
        {
            assembly = assembly ?? Assembly.GetCallingAssembly();
            return Path.Combine(
                Path.GetDirectoryName(assembly.Location) ?? string.Empty,
                $"{assembly.GetName().Name}.Resources");
        }

        private static string GetResourcePath(string path, Assembly assembly = null)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            var resourcesDirectory = GetResourcesDirectory(
                assembly ?? Assembly.GetCallingAssembly());
            return Path.Combine(resourcesDirectory, path);
        }

        private static Stream Open(string path, Assembly assembly = null)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            var fullPath = GetResourcePath(path, assembly ?? Assembly.GetCallingAssembly());
            return File.OpenRead(fullPath);
        }

        private struct ResourceKey
        {
            public string Path { get; }
            public Assembly Assembly { get; }

            public ResourceKey(string path, Assembly assembly)
            {
                Path = path;
                Assembly = assembly;
            }
        }

        private static ConcurrentDictionary<ResourceKey, object> LoadedResources { get; } =
            new ConcurrentDictionary<ResourceKey, object>();

        public static T Load<T>(
            string path, Assembly assembly,
            Func<Stream, T> resourceFactory,
            Func<T> fallbackResourceFactory = null)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (resourceFactory == null) throw new ArgumentNullException(nameof(resourceFactory));
            assembly = assembly ?? Assembly.GetCallingAssembly();
            fallbackResourceFactory = fallbackResourceFactory ?? (() => default(T));

            var resourceKey = new ResourceKey(path, assembly);
            if (LoadedResources.TryGetValue(resourceKey, out var cachedResource))
                return (T)cachedResource;

            lock (LoadedResources)
            {
                if (LoadedResources.TryGetValue(resourceKey, out cachedResource))
                    return (T)cachedResource;

                T resource;
                try
                {
                    using (var stream = Open(path, assembly))
                    {
                        resource = resourceFactory.Invoke(stream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    resource = fallbackResourceFactory.Invoke();
                }

                LoadedResources[resourceKey] = resource;

                return resource;
            }
        }
    }
}
