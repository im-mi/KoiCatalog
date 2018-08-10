using System;
using System.Threading;
using System.Windows;
using KoiCatalog.App;
using KoiCatalog.App.Data;
using KoiCatalog.Data;
using KoiCatalog.Plugins;
using KoiCatalog.Plugins.Koikatu;
using FileInfo = KoiCatalog.Plugins.FileIO.FileInfo;

namespace KoiCatalog.Gui
{
    public partial class ResultsDocument
    {
        private MainWindow Application { get; }

        private static readonly ComponentTypeCode[] DefaultComponentFilter =
        {
            FileInfo.TypeCode,
            KoikatuCharacterCard.TypeCode,
        };

        public Query Query { get; } =
            new Query(DefaultComponentFilter, Array.Empty<QueryParameter>());
        public DataTable QueryResults { get; } = new DataTable();
        public QueryFormat QueryFormat { get; } = new QueryFormat();
        public QueryEditor QueryEditor { get; } = new QueryEditor();
        public DatabaseStatus DatabaseStatus { get; } = new DatabaseStatus();
        public Stats Stats { get; } = new Stats();

        public Database Database { get; } = new Database(QueryUtil.OmniQueryHandler);
        public DirectDatabaseConnection DatabaseConnection { get; } =
            new DirectDatabaseConnection();

        public QueryCleanupService QueryCleanupService { get; } =
            new QueryCleanupService();
        public QueryToQueryResultsService QueryResultsService { get; } =
            new QueryToQueryResultsService();
        public DatabaseToQueryFormatService QueryFormatService { get; } =
            new DatabaseToQueryFormatService();
        public DatabaseToDatabaseStatusService DatabaseStatusService { get; } =
            new DatabaseToDatabaseStatusService();
        public DatabaseToStatsService StatsService { get; } =
            new DatabaseToStatsService();
        public QueryFormatToQueryEditorService QueryEditorService { get; } =
            new QueryFormatToQueryEditorService();

        private static string CacheDirectory { get; } =
            KoiCatalogApp.Current.GetAppRelativePath("Cache");

        public string DatabaseDirectory
        {
            get => (string)GetValue(DatabaseDirectoryProperty);
            set => SetValue(DatabaseDirectoryProperty, value);
        }
        public static readonly DependencyProperty DatabaseDirectoryProperty =
            DependencyProperty.Register(
                nameof(DatabaseDirectory), typeof(string), typeof(ResultsDocument),
                new PropertyMetadata(DatabaseDirectoryChanged));
        
        private static void DatabaseDirectoryChanged(
            DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var resultsDocument = (ResultsDocument)d;
            resultsDocument.Title = resultsDocument.DatabaseDirectory;
            resultsDocument.BeginLoadDatabase();
        }

        internal ResultsDocument() : this(null) { }

        public ResultsDocument(MainWindow application)
        {
            Application = application;

            DatabaseConnection.Database = Database;

            QueryEditor.Query = Query;

            QueryCleanupService.Query = Query;
            QueryCleanupService.DatabaseConnection = DatabaseConnection;

            QueryResultsService.QueryResults = QueryResults;
            QueryResultsService.Query = Query;
            QueryResultsService.DatabaseConnection = DatabaseConnection;

            QueryFormatService.QueryFormat = QueryFormat;
            QueryFormatService.ComponentFilter = Query.ComponentFilter;
            QueryFormatService.DatabaseConnection = DatabaseConnection;

            DatabaseStatusService.DatabaseStatus = DatabaseStatus;
            DatabaseStatusService.DatabaseConnection = DatabaseConnection;

            StatsService.Stats = Stats;
            StatsService.DatabaseConnection = DatabaseConnection;

            QueryEditorService.QueryEditor = QueryEditor;
            QueryEditorService.QueryFormat = QueryFormat;

            InitializeComponent();

            System.Windows.Application.Current.Exit += ApplicationOnExit;

            DatabaseConnection.ConnectAsync().Wait();
        }

        private void ApplicationOnExit(object sender, ExitEventArgs exitEventArgs)
        {
            if (DatabaseConnection.IsConnected)
                DatabaseConnection.DisconnectAsync().Wait();
        }

        public void BeginLoadDatabase(
            DatabaseLoadMode mode = DatabaseLoadMode.Auto)
        {
            Application.CancelAndWaitForAllCurrentTasks();

            // Clear results immediately.
            // This prevents old results from showing up for a moment when switching to this document.
            QueryResultsService.ClearAndCancel();
            QueryFormatService.ClearAndCancel();
            StatsService.ClearAndCancel();

            var cancellationTokenSource = new CancellationTokenSource();
            var task = DatabaseConnection.LoadAsync(
                DatabaseDirectory, CacheDirectory,
                mode: mode,
                cancellationToken: cancellationTokenSource.Token);
            Application.CurrentTasks.Add(new CancellableTask(task, cancellationTokenSource));
        }
    }
}
