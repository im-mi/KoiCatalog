using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KoiCatalog.Data
{
    public sealed class Stat : INotifyPropertyChanged
    {
        public object Key
        {
            get => _key;
            set
            {
                _key = value;
                OnPropertyChanged();
            }
        }
        private object _key;

        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }
        private int _value;

        public StatsCategory Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged();
            }
        }
        private StatsCategory _category;

        public Stat() { }

        public Stat(object key, int value, StatsCategory category)
        {
            Key = key;
            Value = value;
            Category = category;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
