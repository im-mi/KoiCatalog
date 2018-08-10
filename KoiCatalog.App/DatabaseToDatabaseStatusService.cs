using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;
using KoiCatalog.Data;

namespace KoiCatalog.App
{
    public sealed class DatabaseToDatabaseStatusService : INotifyPropertyChanged
    {
        public IDatabaseConnection DatabaseConnection
        {
            get => _databaseConnection;
            set
            {
                if (value == _databaseConnection) return;
                
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

                Reset();
                BeginRefresh();
                NotifyPropertyChanged();
            }
        }
        private IDatabaseConnection _databaseConnection;

        public DatabaseStatus DatabaseStatus
        {
            get => _databaseStatus;
            set
            {
                if (value == _databaseStatus) return;
                _databaseStatus = value;
                Reset();
                BeginRefresh();
                NotifyPropertyChanged();
            }
        }
        private DatabaseStatus _databaseStatus = new DatabaseStatus();

        private void Reset()
        {
            Cancel();
            if (DatabaseStatus != null)
            {
                DatabaseStatus.TotalDatabaseItemCount = 0;
                DatabaseStatus.State = DatabaseStatusState.Ready;
                DatabaseStatus.Progress = null;
            }
        }

        private void DatabaseConnectionOnConnected(object sender, EventArgs e)
        {
            KoiCatalogApp.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                Reset();
                BeginRefresh();
            }));
        }

        private void DatabaseConnectionOnEventOccurred(object sender, DatabaseEventEventArgs e)
        {
            KoiCatalogApp.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (DatabaseStatus == null) return;

                DatabaseStatus.State = DatabaseStatus.DatabaseEventTypeToStatusStatus(e.Event.EventType) ?? DatabaseStatus.State;

                if (e.Event.EventType == DatabaseEventType.LoadStart)
                {
                    DatabaseStatus.Events.Clear();
                    BeginRefresh();
                }
                if (e.Event.EventType == DatabaseEventType.LoadProgress)
                {
                    DatabaseStatus.Progress = e.Event.Progress;
                }
                if (e.Event.EventType == DatabaseEventType.LoadEnd)
                {
                    DatabaseStatus.Progress = null;
                }

                DatabaseStatus.Events.Add(e.Event);
            }));
        }

        public void BeginRefresh()
        {
            Cancel();
            if (DatabaseStatus == null) return;

            DatabaseStatus.TotalDatabaseItemCount = 0;

            if (DatabaseConnection == null) return;
            if (!DatabaseConnection.IsConnected) return;

            RefreshCancellationToken = new CancellationTokenSource();
            var cancellationToken = RefreshCancellationToken.Token;

            var newTotalDatabaseItems = 0;
            var task = DatabaseConnection.SubmitQueryAsync(new Query(),
                result => Interlocked.Increment(ref newTotalDatabaseItems), cancellationToken);

            const int updateInterval = 500;

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(updateInterval);
            timer.Tick += (sender, e) =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    timer.Stop();
                    return;
                }
                
                if (task.IsCompleted)
                    timer.Stop();
                else
                    timer.Start();

                DatabaseStatus.TotalDatabaseItemCount = newTotalDatabaseItems;
            };
            timer.Start();
        }

        public void Cancel()
        {
            RefreshCancellationToken?.Cancel();
        }

        private CancellationTokenSource RefreshCancellationToken { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
