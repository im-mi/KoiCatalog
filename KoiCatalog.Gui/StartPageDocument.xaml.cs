using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using KoiCatalog.App;
using KoiCatalog.Plugins.FileIO;

namespace KoiCatalog.Gui
{
    public partial class StartPageDocument
    {
        public ObservableCollection<DefaultDirectory> DefaultDirectories { get; } =
            new ObservableCollection<DefaultDirectory>();

        private MainWindow Application { get; }

        public StartPageDocument() : this(null) { }

        public StartPageDocument(MainWindow application)
        {
            InitializeComponent();
            Application = application;
            Title = "Start Page";

            DefaultDirectories.AddRange(FileIOUtil.OmniDefaultDirectoryHandler.GetDirectories());
        }

        private async void LoadDirectoryButton_OnClick(object sender, RoutedEventArgs e)
        {
            var defaultDirectory = (DefaultDirectory)((FrameworkElement)sender).DataContext;
            await Application.LoadDirectoryAsync(defaultDirectory.Location);
        }
        
        private async void OpenDirectoryCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            await Application.TryOpenDirectoryDialogAsync();
        }

        private void AllowIfActionsEnabled_OnPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Application == null || Application.ActionLock.IsLocked)
            {
                e.CanExecute = false;
                e.Handled = true;
                e.ContinueRouting = false;
            }
        }
    }
}
