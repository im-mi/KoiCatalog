using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;
using KoiCatalog.Data;
using KoiCatalog.Plugins;

namespace KoiCatalog.App
{
    public sealed class DatabaseToStatsService : INotifyPropertyChanged
    {
        public IDatabaseConnection DatabaseConnection
        {
            get => _databaseConnection;
            set
            {
                if (value == _databaseConnection) return;

                if (_databaseConnection != null)
                {
                    _databaseConnection.EventOccurred -= DatabaseOnEventOccurred;
                    _databaseConnection.Connected -= DatabaseConnectionOnConnected;
                }

                _databaseConnection = value;

                if (_databaseConnection != null)
                {
                    _databaseConnection.EventOccurred += DatabaseOnEventOccurred;
                    _databaseConnection.Connected += DatabaseConnectionOnConnected;
                }

                BeginRefresh();
                OnPropertyChanged();
            }
        }
        private IDatabaseConnection _databaseConnection;

        public Stats Stats
        {
            get => _stats;
            set
            {
                if (value == _stats) return;
                _stats = value;
                BeginRefresh();
                OnPropertyChanged();
            }
        }
        private Stats _stats = new Stats();

        private void DatabaseConnectionOnConnected(object sender, EventArgs eventArgs)
        {
            KoiCatalogApp.Current.Dispatcher.BeginInvoke(new Action(BeginRefresh));
        }

        private void DatabaseOnEventOccurred(object sender, DatabaseEventEventArgs e)
        {
            if (e.Event.EventType == DatabaseEventType.LoadStart)
            {
                KoiCatalogApp.Current.Dispatcher.BeginInvoke(new Action(BeginRefresh));
            }
        }

        private StatsLoader StatsLoader { get; } = new StatsLoader();

        private StatsHandler[] StatsHandlers { get; } =
            PluginManager.GetNewTypes<StatsHandler>()
            .Select(Activator.CreateInstance)
            .Cast<StatsHandler>()
            .ToArray();

        private void AddStatisticsForEntity(IReadOnlyEntity entity)
        {
            if (Stats == null) return;

            // Todo: Generate stats server-side.
            // Todo: Make entity read-only.
            StatsLoader.Entity = entity;
            StatsLoader.Stats = Stats;

            foreach (var statsHandler in StatsHandlers)
            {
                try
                {
                    statsHandler.GetStats(StatsLoader);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public void BeginRefresh()
        {
            ClearAndCancel();

            if (Stats == null) return;
            if (DatabaseConnection == null) return;
            if (!DatabaseConnection.IsConnected) return;

            RefreshCancellationToken = new CancellationTokenSource();
            var cancellationToken = RefreshCancellationToken.Token;

            var results = new BlockingCollection<QueryResult>();
            var task = DatabaseConnection.SubmitQueryAsync(new Query(), result =>
                    results.Add(result, cancellationToken), cancellationToken)
                .ContinueWith(t => results.CompleteAdding(), cancellationToken);

            const int updateInterval = 100;
            const int maximumResultsPerUpdate = 453;

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(updateInterval);
            timer.Tick += (sender, e) =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    timer.Stop();
                    return;
                }

                for (var i = 0; i < maximumResultsPerUpdate; i++)
                {
                    if (results.TryTake(out var result))
                        AddStatisticsForEntity(result.Data);
                    else
                        break;
                }

                if (!task.IsCompleted || results.Count > 0 || !results.IsAddingCompleted)
                {
                    timer.Start();
                }
                else
                {
                    timer.Stop();
                }
            };
            timer.Start();
        }

        public void Clear()
        {
            Stats?.Clear();
        }

        public void Cancel()
        {
            RefreshCancellationToken?.Cancel();
        }

        public void ClearAndCancel()
        {
            Clear();
            Cancel();
        }

        private CancellationTokenSource RefreshCancellationToken { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
