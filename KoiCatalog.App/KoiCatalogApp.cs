using System;
using System.IO;
using System.Windows.Threading;
using KoiCatalog.Plugins;
using SimpleErrorReporter;

namespace KoiCatalog.App
{
    public sealed class KoiCatalogApp
    {
        public static KoiCatalogApp Current { get; private set; }
        private static readonly object CurrentAppLock = new object();

        public KoiCatalogApp(Dispatcher dispatcher, string baseDirectory)
        {
            if (dispatcher == null) throw new ArgumentNullException(nameof(dispatcher));
            if (baseDirectory == null) throw new ArgumentNullException(nameof(baseDirectory));

            lock (CurrentAppLock)
            {
                if (Current != null)
                {
                    throw new InvalidOperationException();
                }
                Current = this;
            }

            Dispatcher = dispatcher;
            BaseDirectory = baseDirectory;

            if (GlobalOptions.ErrorReportingEnabled)
            {
                ErrorReporter.LogDirectory = Path.Combine(BaseDirectory, "Error Reports");
                ErrorReporter.Install();
            }

            PluginManager.RegisterPluginDirectory(GetAppRelativePath(@"Resources\CorePlugins"));
            PluginManager.RegisterPluginDirectory(GetAppRelativePath("Plugins"));
            // This hack allows pre-installed plugins to be loaded from the base directory.
            // That way it's not necessary to manually move them to a plugins directory
            // or deal with finicky post-build scripts.
            PluginManager.RegisterPluginDirectory(GetAppRelativePath(""));
        }

        public Dispatcher Dispatcher { get; }
        public string BaseDirectory { get; }

        public string GetAppRelativePath(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            return Path.Combine(BaseDirectory, path);
        }
    }
}
