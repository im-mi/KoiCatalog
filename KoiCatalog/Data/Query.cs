using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using KoiCatalog.Util;

namespace KoiCatalog.Data
{
    public sealed class Query : IEnumerable<QueryParameter>, ICloneable<Query>
    {
        public delegate void ChangedEventHandler(object sender, EventArgs e);
        public event ChangedEventHandler Changed;

        public ObservableCollection<ComponentTypeCode> ComponentFilter { get; } =
            new ObservableCollection<ComponentTypeCode>();
        private List<QueryParameter> Parameters { get; } = new List<QueryParameter>();

        public Query(
            IEnumerable<ComponentTypeCode> componentFilter,
            IEnumerable<QueryParameter> parameters) : this()
        {
            foreach (var typeCode in componentFilter ?? Array.Empty<ComponentTypeCode>())
                ComponentFilter.Add(typeCode);
            foreach (var parameter in parameters ?? Array.Empty<QueryParameter>())
                Add(parameter);
        }

        public Query()
        {
            ComponentFilter.CollectionChanged += ComponentFilterOnCollectionChanged;
        }

        private void ComponentFilterOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Add(QueryParameter parameter)
        {
            if (parameter == null) throw new ArgumentNullException(nameof(parameter));
            Parameters.Add(parameter);
            parameter.Changed += ParameterOnChanged;
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void Clear()
        {
            foreach (var parameter in Parameters)
                parameter.Changed -= ParameterOnChanged;
            Parameters.Clear();
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void SetValue(QueryParameterTemplate template, object value)
        {
            CreateParameter(template).Value = value;
        }
        
        public void SelectValue(QueryParameterTemplate template, object value)
        {
            var parameter = CreateParameter(template);
            if (parameter.Selection.Contains(value)) return;
            parameter.Selection.Add(value);
        }

        public void DeselectValue(QueryParameterTemplate template, object value)
        {
            var parameter = CreateParameter(template);
            parameter.Selection.Remove(value);
        }

        public bool TryGetParameter(QueryParameterTemplate template, out QueryParameter parameter)
        {
            parameter = Parameters.FirstOrDefault(i => i.Template == template);
            return parameter != null;
        }

        private QueryParameter CreateParameter(QueryParameterTemplate template)
        {
            if (!TryGetParameter(template, out var parameter))
            {
                parameter = new QueryParameter(template, null);
                Add(parameter);
            }
            return parameter;
        }

        private void ParameterOnChanged(object sender, EventArgs e)
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public bool Filter(IReadOnlyEntity entity, QueryHandler queryHandler)
        {
            if (queryHandler == null) throw new ArgumentNullException(nameof(queryHandler));
            return Parameters.All(parameter => parameter.Filter(entity, queryHandler));
        }

        public IEnumerator<QueryParameter> GetEnumerator() => Parameters.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public Query Clone()
        {
            return new Query(ComponentFilter, Parameters.Select(i => i.Clone()));
        }

        object ICloneable.Clone() => Clone();
    }
}
