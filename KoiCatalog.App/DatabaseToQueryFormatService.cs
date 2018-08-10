using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;
using KoiCatalog.Data;

namespace KoiCatalog.App
{
    public sealed class DatabaseToQueryFormatService : INotifyPropertyChanged
    {
        public IDatabaseConnection DatabaseConnection
        {
            get => _databaseConnection;
            set
            {
                if (_databaseConnection == value) return;

                if (_databaseConnection != null)
                {
                    _databaseConnection.Connected -= DatabaseConnectionOnConnected;
                    _databaseConnection.EventOccurred -= DatabaseConnectionOnEventOccurred;
                }

                _databaseConnection = value;

                if (_databaseConnection != null)
                {
                    _databaseConnection.Connected += DatabaseConnectionOnConnected;
                    _databaseConnection.EventOccurred += DatabaseConnectionOnEventOccurred;
                }

                BeginRefreshQueryFormat();
                OnPropertyChanged();
            }
        }
        private IDatabaseConnection _databaseConnection;

        public ObservableCollection<ComponentTypeCode> ComponentFilter
        {
            get => _componentFilter;
            set
            {
                if (value == _componentFilter) return;

                if (_componentFilter != null)
                {
                    ComponentFilter.CollectionChanged -= ComponentsOnCollectionChanged;
                }

                _componentFilter = value;

                if (_componentFilter != null)
                {
                    ComponentFilter.CollectionChanged += ComponentsOnCollectionChanged;
                }

                BeginRefreshQueryFormat();
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ComponentTypeCode> _componentFilter =
            new ObservableCollection<ComponentTypeCode>();

        public QueryFormat QueryFormat
        {
            get => _queryFormat;
            set
            {
                if (value == _queryFormat) return;
                _queryFormat = value;
                BeginRefreshQueryFormat();
                OnPropertyChanged();
            }
        }
        private QueryFormat _queryFormat = new QueryFormat();

        public DatabaseToQueryFormatService()
        {
            ComponentFilter = new ObservableCollection<ComponentTypeCode>();
        }

        private void ComponentsOnCollectionChanged(object o, NotifyCollectionChangedEventArgs e)
        {
            BeginRefreshQueryFormat();
        }

        private void DatabaseConnectionOnEventOccurred(object sender, DatabaseEventEventArgs e)
        {
            if (e.Event.EventType == DatabaseEventType.LoadStart)
            {
                KoiCatalogApp.Current.Dispatcher.BeginInvoke(new Action(BeginRefreshQueryFormat));
            }
        }

        private void DatabaseConnectionOnConnected(object sender, EventArgs eventArgs)
        {
            KoiCatalogApp.Current.Dispatcher.BeginInvoke(new Action(BeginRefreshQueryFormat));
        }

        public void Clear()
        {
            QueryFormat?.Clear();
        }

        public void Cancel()
        {
            QueryFormatRequestCancellationToken?.Cancel();
        }

        public void ClearAndCancel()
        {
            Clear();
            Cancel();
        }

        private void BeginRefreshQueryFormat()
        {
            ClearAndCancel();

            if (DatabaseConnection == null) return;
            if (!DatabaseConnection.IsConnected) return;
            if (ComponentFilter == null) return;

            QueryFormatRequestCancellationToken = new CancellationTokenSource();
            var cancellationToken = QueryFormatRequestCancellationToken.Token;

            var results = new BlockingCollection<QueryFormat.Action>();
            var task = DatabaseConnection.RequestQueryFormatAsync(
                    ComponentFilter,
                    result => results.Add(result, cancellationToken), cancellationToken)
                .ContinueWith(t => results.CompleteAdding(), cancellationToken);

            const int updateInterval = 100;
            const int maximumResultsPerUpdate = 5;

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
                        QueryFormat?.PerformAction(result);
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

        private CancellationTokenSource QueryFormatRequestCancellationToken { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
