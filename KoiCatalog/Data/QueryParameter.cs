using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using KoiCatalog.Util;

namespace KoiCatalog.Data
{
    public sealed class QueryParameter : ICloneable<QueryParameter>
    {
        public event EventHandler Changed;

        public QueryParameterTemplate Template
        {
            get => _template;
            set
            {
                if (value == _template) return;
                _template = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
        private QueryParameterTemplate _template;

        public object Value
        {
            get => _value;
            set
            {
                if (value == _value) return;
                _value = value;
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
        private object _value;

        public ObservableCollection<object> Selection { get; } = new ObservableCollection<object>();

        public QueryParameter()
        {
            Selection.CollectionChanged += SelectionOnCollectionChanged;
        }
        
        public QueryParameter(QueryParameterTemplate template, object value, IEnumerable<object> selection = null) : this()
        {
            Template = template;
            Value = value;
            if (selection != null)
            {
                foreach (var item in selection)
                    Selection.Add(item);
            }
        }

        private void SelectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public bool Filter(IReadOnlyEntity entity, QueryHandler queryHandler)
        {
            if (queryHandler == null) throw new ArgumentNullException(nameof(queryHandler));
            return queryHandler.Filter(entity, this);
        }

        public QueryParameter Clone()
        {
            return new QueryParameter(Template, Value, Selection);
        }

        object ICloneable.Clone() => Clone();
    }
}
