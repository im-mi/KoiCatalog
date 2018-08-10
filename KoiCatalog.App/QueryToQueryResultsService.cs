using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;
using KoiCatalog.App.Data;
using KoiCatalog.Data;

namespace KoiCatalog.App
{
    public sealed class QueryToQueryResultsService : INotifyPropertyChanged
    {
        public IDatabaseConnection DatabaseConnection
        {
            get => _databaseConnection;
            set
            {
                if (value == _databaseConnection) return;
                if (_databaseConnection != null)
                    _databaseConnection.Connected -= Database_Connected;
                if (_databaseConnection == value) return;
                _databaseConnection = value;
                if (_databaseConnection != null)
                    _databaseConnection.Connected += Database_Connected;
                Invalidate();
                OnPropertyChanged();
            }
        }
        private IDatabaseConnection _databaseConnection;

        public Query Query
        {
            get => _query;
            set
            {
                if (value == _query) return;
                if (_query != null)
                    _query.Changed -= QueryChanged;
                _query = value;
                if (_query != null)
                    _query.Changed += QueryChanged;
                OnPropertyChanged();
                Invalidate();
            }
        }
        private Query _query;

        public DataTable QueryResults
        {
            get => _queryResults;
            set
            {
                if (value == _queryResults) return;
                _queryResults = value;
                Invalidate();
                OnPropertyChanged();
            }
        }
        private DataTable _queryResults = new DataTable();

        public bool IsRefreshing
        {
            get => _isRefreshing;
            private set
            {
                if (value == _isRefreshing) return;
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }
        private bool _isRefreshing;

        public TimeSpan RefreshDelay
        {
            get => _refreshDelay;
            set
            {
                if (value == _refreshDelay) return;
                _refreshDelay = value;
                OnPropertyChanged();
            }
        }
        private TimeSpan _refreshDelay = TimeSpan.FromMilliseconds(400);

        public bool ResetRefreshDelayOnInvalidate
        {
            get => _resetRefreshDelayOnInvalidate;
            set
            {
                if (value == _resetRefreshDelayOnInvalidate) return;
                _resetRefreshDelayOnInvalidate = value;
                OnPropertyChanged();
            }
        }
        private bool _resetRefreshDelayOnInvalidate = true;

        private void Database_Connected(object sender, EventArgs e)
        {
            KoiCatalogApp.Current.Dispatcher.BeginInvoke(new Action(Invalidate));
        }

        private void QueryChanged(object sender, EventArgs eventArgs)
        {
            Invalidate();
        }

        /// <summary>
        /// Invalidate this collection, causing it to begin refreshing after the amount of time indicated by
        /// <see cref="RefreshDelay"/> has passed. If <see cref="Invalidate"/> is called again while waiting and
        /// <see cref="ResetRefreshDelayOnInvalidate"/> is also enabled, then waiting will begin anew.
        /// <seealso cref="Refresh"/>
        /// </summary>
        public void Invalidate()
        {
            if (Timer != null)
            {
                if (ResetRefreshDelayOnInvalidate)
                {
                    Timer.Interval = RefreshDelay;
                    Timer.Start();
                }
            }
            else
            {
                Timer = new DispatcherTimer();
                Timer.Interval = RefreshDelay;
                Timer.Tick += (sender, e) =>
                {
                    Refresh();
                    Timer.Stop();
                    Timer = null;
                };
                Timer.Start();
            }
        }

        public void Clear()
        {
            QueryResults?.Clear();
        }

        /// <summary>
        /// Cancels the refresh that is in progress. If no refresh is in progress, then no action is taken.
        /// </summary>
        public void Cancel()
        {
            if (LatestQueryCancellationToken == null) return;
            LatestQueryCancellationToken.Cancel();
            LatestQueryCancellationToken = null;
            IsRefreshing = false;
        }

        public void ClearAndCancel()
        {
            Clear();
            Cancel();
        }

        /// <summary>
        /// Refresh this collection, causing it to begin updating immediately.
        /// <seealso cref="Invalidate"/>
        /// </summary>
        public void Refresh()
        {
            ClearAndCancel();
            RefreshColumns();

            if (DatabaseConnection == null) return;
            if (!DatabaseConnection.IsConnected) return;
            if (Query == null) return;

            IsRefreshing = true;

            LatestQueryCancellationToken = new CancellationTokenSource();
            var cancellationToken = LatestQueryCancellationToken.Token;

            var results = new BlockingCollection<QueryResult>();
            var task = DatabaseConnection.SubmitQueryAsync(Query,
                    result => results.Add(result, cancellationToken), cancellationToken)
                .ContinueWith(t => results.CompleteAdding(), cancellationToken);

            const int updateInterval = 100;
            const int maximumResultsPerUpdate = 53;

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
                        Add(result.Data);
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
                    IsRefreshing = false;
                }
            };
            timer.Start();
        }

        private void Add(IReadOnlyEntity entity)
        {
            if (QueryResults == null) return;

            var row = QueryResults.NewRow();
            foreach (var i in QueryResults.Columns.Select((item, index) => new { item.Property, index }))
            {
                var component = entity.GetComponent(ComponentTypeCode.Get(i.Property.DeclaringType));
                Trace.Assert(component != null);

                object value;
                try
                {
                    value = i.Property.GetValue(component);
                }
                catch
                {
                    value = null;
                }
                row[i.index] = value;
            }
            QueryResults.Rows.Add(row);
        }

        private ComponentTypeCode[] CurrentColumnTypes = Array.Empty<ComponentTypeCode>();

        private void RefreshColumns()
        {
            if (QueryResults == null) return;

            var newColumns = Query?.ComponentFilter.ToArray() ?? Array.Empty<ComponentTypeCode>();
            if (CurrentColumnTypes.SequenceEqual(newColumns))
                return;

            CurrentColumnTypes = newColumns;
            QueryResults.Columns.Clear();

            var properties = CurrentColumnTypes
                .SelectMany(i => i.ComponentType.GetProperties(BindingFlags.Instance | BindingFlags.Public));

            foreach (var property in properties)
            {
                var column = new DataColumn(property);
                QueryResults.Columns.Add(column);
            }
        }

        private CancellationTokenSource LatestQueryCancellationToken { get; set; }
        private DispatcherTimer Timer { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
