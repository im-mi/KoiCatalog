using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KoiCatalog.Data;

namespace KoiCatalog.App
{
    public sealed class QueryCleanupService : INotifyPropertyChanged
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
                _query = value;
                OnPropertyChanged();
            }
        }
        private Query _query;

        private void DatabaseConnectionOnEventOccurred(
            object sender, DatabaseEventEventArgs e)
        {
            if (e.Event.EventType == DatabaseEventType.LoadStart)
            {
                KoiCatalogApp.Current.Dispatcher.BeginInvoke(new Action(Query.Clear));
            }
        }

        private void DatabaseConnectionOnConnected(
            object sender, EventArgs e)
        {
            KoiCatalogApp.Current.Dispatcher.BeginInvoke(new Action(Query.Clear));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
