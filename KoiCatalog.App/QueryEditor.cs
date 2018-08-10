using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using KoiCatalog.Data;

namespace KoiCatalog.App
{
    public sealed class QueryEditor : ReadOnlyObservableCollection<QueryEditorParameter>
    {
        private Query _query;

        public Query Query
        {
            get => _query;
            set
            {
                _query = value;
                OnPropertyChanged();
            }
        }

        public QueryEditorParameter CreateParameter(QueryParameterTemplate template)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            var formattedParameter = this.FirstOrDefault(i => i.Template == template);
            if (formattedParameter == null)
            {
                formattedParameter = new QueryEditorParameter(this, template);
                Items.Add(formattedParameter);
            }
            return formattedParameter;
        }

        public void Clear()
        {
            Items.Clear();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public QueryEditor() : base(new ObservableCollection<QueryEditorParameter>())
        {
        }
    }
}
