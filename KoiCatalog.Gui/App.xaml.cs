using System.Windows;
using SimpleErrorReporter;

namespace KoiCatalog.Gui
{
    public partial class App
    {
        private async void App_OnStartup(object sender, StartupEventArgs e)
        {
            ErrorReporter.ShowError += ErrorReporterOnShowError;
            await AppStartup.Start();
        }

        private static void ErrorReporterOnShowError(object sender, MessageEventArgs e)
        {
            MessageBox.Show(
                e.Message,
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
