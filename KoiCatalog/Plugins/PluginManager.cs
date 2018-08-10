using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace KoiCatalog.Plugins
{
    public static class PluginManager
    {
        public static void EnsurePluginsLoaded()
        {
            if (PluginsLoaded) return;

            lock (PluginLoadLock)
            {
                if (PluginsLoaded) return;

                foreach (var directory in PluginDirectories)
                {
                    try
                    {
                        LoadPluginsDirectory(directory);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                PluginsLoaded = true;
            }
        }

        private static volatile bool PluginsLoaded;
        private static readonly object PluginLoadLock = new object();

        private static readonly IReadOnlyList<string> PluginExtensions = new List<string>
        {
            ".dll",
            ".exe",
        }.AsReadOnly();

        private static bool IsPluginPath(string path)
        {
            var extension = Path.GetExtension(path);
            return PluginExtensions.Any(i => string.Equals(i, extension, StringComparison.InvariantCultureIgnoreCase));
        }

        private static void LoadPluginsDirectory(string directory)
        {
            if (!Directory.Exists(directory)) return;

            foreach (var fileName in Directory.GetFiles(directory))
            {
                try
                {
                    if (!IsPluginPath(fileName)) continue;
                    var filePath = Path.Combine(directory, fileName);
                    Assembly.LoadFrom(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Gets all types with a public parameterless constructor that are assignable to <see cref="T"/>.
        /// </summary>
        /// <remarks>
        /// This is equivalent to the "new" generic type constraint.
        /// Additionally, <see cref="EnsurePluginsLoaded"/> is automatically called before performing the operation.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<Type> GetNewTypes<T>()
        {
            EnsurePluginsLoaded();
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(i => i.GetTypes())
                .Where(i => typeof(T).IsAssignableFrom(i))
                .Where(i => i.GetConstructor(Type.EmptyTypes) != null)
                .ToList();
        }

        public static void RegisterPluginDirectory(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            PluginDirectories.Add(path);
        }

        private static ConcurrentBag<string> PluginDirectories { get; } = new ConcurrentBag<string>();
    }
}
