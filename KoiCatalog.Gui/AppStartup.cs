using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using KoiCatalog.App;

namespace KoiCatalog.Gui
{
    static class AppStartup
    {
        public static async Task Start()
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var app = new KoiCatalogApp(Application.Current.Dispatcher, assemblyDirectory);

            var mainWindow = new MainWindow();
            foreach (var directory in GlobalOptions.StartupDirectories.Take(1))
                await mainWindow.LoadDirectoryAsync(new Uri(directory));
            mainWindow.Show();
        }
    }
}
