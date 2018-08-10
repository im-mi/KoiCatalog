using Ookii.Dialogs.Wpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using KoiCatalog.App;
using MahApps.Metro.Controls.Dialogs;

namespace KoiCatalog.Gui
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        public ActionLockTarget ActionLock { get; } = new ActionLockTarget();
        public ReadOnlyObservableCollection<Document> Documents =>
            _readOnlyDocuments ?? (_readOnlyDocuments = new ReadOnlyObservableCollection<Document>(_documents));
        private ReadOnlyObservableCollection<Document> _readOnlyDocuments;
        private readonly ObservableCollection<Document> _documents = new ObservableCollection<Document>();

        public Document CurrentDocument
        {
            get => (Document)GetValue(CurrentDocumentProperty);
            set => SetValue(CurrentDocumentProperty, value);
        }
        public static readonly DependencyProperty CurrentDocumentProperty =
            DependencyProperty.Register(nameof(CurrentDocument), typeof(Control), typeof(MainWindow),
                new PropertyMetadata(CurrentDocumentChangedCallback));

        private static void CurrentDocumentChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (MainWindow)d;
            self.CurrentDocumentPanel.Content = e.NewValue;

            self.CurrentDocumentPanel.Focus();
            Keyboard.Focus(self.CurrentDocumentPanel);
            CommandManager.InvalidateRequerySuggested();
        }

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.Exit += ApplicationOnExit;
            CurrentDocument = GetStartPageDocument(true);
        }
        
        private void ApplicationOnExit(object sender, ExitEventArgs exitEventArgs)
        {
            CancelAndWaitForAllCurrentTasks();
        }

        /// <summary>
        /// Attempt to cancel any current tasks with a confirmation dialog.
        /// </summary>
        /// <returns>Whether the tasks were cancelled or weren't running.</returns>
        private async Task<bool> ConfirmAllTasksCanceledAsync(bool cancel = true)
        {
            using (ActionLock.AcquireLock())
            {
                if (!CurrentTasks.IsAnyTaskRunning) return true;

                if (await this.ShowCustomMessageAsync("Warning", "This will cancel any tasks in progress. Continue?",
                        MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                {
                    if (cancel)
                        CancelAndWaitForAllCurrentTasks();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void CancelAndWaitForAllCurrentTasks()
        {
            CurrentTasks.CancelAll();

            try
            {
                // Todo: Don't block GUI thread.
                CurrentTasks.WaitAll();
            }
            catch (AggregateException)
            {
            }
        }

        public CancellableTaskGroup CurrentTasks { get; } = new CancellableTaskGroup();

        public async Task LoadDirectoryAsync(Uri uri)
        {
            using (ActionLock.AcquireLock())
            {
                var resultsDocument = GetResultsDocument(true);
                if (resultsDocument.DatabaseDirectory != uri.LocalPath)
                {
                    if (!await ConfirmAllTasksCanceledAsync()) return;
                    resultsDocument.DatabaseDirectory = uri.LocalPath;
                }
                CurrentDocument = resultsDocument;
            }
        }

        public async Task TryOpenDirectoryDialogAsync()
        {
            using (ActionLock.AcquireLock())
            {
                // Todo: Try to share code with LoadDirectoryAsync.
                if (!await ConfirmAllTasksCanceledAsync(false)) return;
                var dialog = new VistaFolderBrowserDialog();
                if (dialog.ShowDialog() != true) return;
                var resultsDocument = GetResultsDocument(true);
                resultsDocument.DatabaseDirectory = dialog.SelectedPath;
                CurrentDocument = resultsDocument;
            }
        }

        private async void OpenDirectoryCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            await TryOpenDirectoryDialogAsync();
        }
        
        private void DatabaseCommand_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResultsDocument != null;
        }
        
        private async void RebuildCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (ResultsDocument == null) return;

            using (ActionLock.AcquireLock())
            {
                if (await this.ShowCustomMessageAsync("Warning", "The entire database will be rebuilt. Continue?",
                        MessageDialogStyle.AffirmativeAndNegative) != MessageDialogResult.Affirmative)
                {
                    return;
                }
                if (!await ConfirmAllTasksCanceledAsync()) return;
                ResultsDocument.BeginLoadDatabase(DatabaseLoadMode.Rebuild);
                CurrentDocument = ResultsDocument;
            }
        }

        private async void RefreshCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ResultsDocument == null) return;

            using (ActionLock.AcquireLock())
            {
                if (!await ConfirmAllTasksCanceledAsync()) return;
                ResultsDocument.BeginLoadDatabase(DatabaseLoadMode.Refresh);
                CurrentDocument = ResultsDocument;
            }
        }

        private async void Window_Closing(object sender, CancelEventArgs e)
        {
            using (ActionLock.AcquireLock())
            {
                e.Cancel = true;
                if (!await ConfirmAllTasksCanceledAsync()) return;
                Application.Current.Shutdown();
            }
        }

        private async void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (ActionLock.AcquireLock())
            {
                if (!await ConfirmAllTasksCanceledAsync()) return;
                Application.Current.Shutdown();
            }
        }

        private async void AboutCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            using (ActionLock.AcquireLock())
            {
                await this.ShowCustomMessageAsync(null, new AboutControl(), MessageDialogStyle.Affirmative);
            }
        }

        private void StartScreenCommand_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CurrentDocument = GetStartPageDocument(true);
        }

        private void CurrentDocumentCloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            CloseDocument(CurrentDocument);
        }

        private StartPageDocument GetStartPageDocument(bool create = false)
        {
            var document = _documents.OfType<StartPageDocument>().FirstOrDefault();
            if (document == null && create)
            {
                document = new StartPageDocument(this);
                _documents.Add(document);
                StartPageDocument = document;
            }
            return document;
        }

        private ResultsDocument GetResultsDocument(bool create = false)
        {
            var document = _documents.OfType<ResultsDocument>().FirstOrDefault();
            if (document == null && create)
            {
                document = new ResultsDocument(this);
                document.IsUserClosable = false;
                _documents.Add(document);
                ResultsDocument = document;
            }
            return document;
        }

        public StartPageDocument StartPageDocument
        {
            get => _startPageDocument;
            private set
            {
                if (ReferenceEquals(value, _startPageDocument)) return;
                _startPageDocument = value;
                OnPropertyChanged();
            }
        }
        private StartPageDocument _startPageDocument;

        public ResultsDocument ResultsDocument
        {
            get => _resultsDocument;
            private set
            {
                if (ReferenceEquals(value, _resultsDocument)) return;
                _resultsDocument = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }
        private ResultsDocument _resultsDocument;

        private void CloseDocument(Document document)
        {
            var documentIndex = _documents.IndexOf(document);
            if (documentIndex < 0) return;

            _documents.Remove(document);
            if (ReferenceEquals(document, StartPageDocument))
                StartPageDocument = null;
            else if (ReferenceEquals(document, ResultsDocument))
                ResultsDocument = null;
            Document nextDocument = null;
            if (_documents.Count > 0)
            {
                nextDocument = _documents[Math.Min(documentIndex, _documents.Count - 1)];
            }
            CurrentDocument = nextDocument;
        }

        private void AllowIfActionsEnabled_OnPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ActionLock.IsLocked)
            {
                e.CanExecute = false;
                e.Handled = true;
                e.ContinueRouting = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DatabaseStatus_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            EventLogFlyout.IsOpen = true;
        }

        private void NavigateTo_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var uri = e.Parameter as Uri ?? (e.OriginalSource as Hyperlink)?.NavigateUri;
            if (uri == null) return;
            try
            {
                if (uri.IsFile)
                {
                    IOUtil.OpenFileFolder(uri);
                }
                else
                {
                    IOUtil.OpenUri(uri);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
