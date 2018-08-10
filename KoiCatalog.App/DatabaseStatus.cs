using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KoiCatalog.Data;

namespace KoiCatalog.App
{
    public sealed class DatabaseStatus : INotifyPropertyChanged
    {
        public ObservableCollection<DatabaseEvent> Events { get; } =
            new ObservableCollection<DatabaseEvent>();

        public Progress Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(LoadingState));
            }
        }
        private Progress _progress;

        public DatabaseStatusState State
        {
            get => _state;
            set
            {
                _state = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(LoadingState));
            }
        }
        private DatabaseStatusState _state = DatabaseStatusState.Ready;

        public DatabaseLoadingState LoadingState
        {
            get
            {
                if (State == DatabaseStatusState.Ready)
                    return DatabaseLoadingState.None;
                if (Progress == null)
                    return DatabaseLoadingState.Indeterminate;
                return DatabaseLoadingState.Normal;
            }
        }

        public int TotalDatabaseItemCount
        {
            get => _totalDatabaseItemCount;
            set
            {
                if (value == _totalDatabaseItemCount) return;
                _totalDatabaseItemCount = value;
                NotifyPropertyChanged();
            }
        }
        private int _totalDatabaseItemCount;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static DatabaseStatusState? DatabaseEventTypeToStatusStatus(DatabaseEventType eventType)
        {
            switch (eventType)
            {
                case DatabaseEventType.SaveStart:
                    return DatabaseStatusState.Saving;
                case DatabaseEventType.ScanStart:
                    return DatabaseStatusState.Scanning;
                case DatabaseEventType.LoadStart:
                case DatabaseEventType.LoadProgress:
                    return DatabaseStatusState.Loading;
                case DatabaseEventType.SaveEnd:
                case DatabaseEventType.ScanEnd:
                case DatabaseEventType.LoadEnd:
                    return DatabaseStatusState.Ready;
                default:
                    return null;
            }
        }
    }
}
