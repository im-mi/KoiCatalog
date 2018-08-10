using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KoiCatalog.App
{
    public sealed class QueryEditorParameterSelectableValue : INotifyPropertyChanged
    {
        public object Value
        {
            get => _value;
            set
            {
                if (ReferenceEquals(value, _value)) return;
                _value = value;
                OnPropertyChanged();
            }
        }
        private object _value;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                if (_isSelected)
                    Owner.QueryEditor.Query?.SelectValue(Owner.Template, Value);
                else
                    Owner.QueryEditor.Query?.DeselectValue(Owner.Template, Value);
                OnPropertyChanged();
            }
        }
        private bool _isSelected;

        public QueryEditorParameter Owner { get; internal set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
