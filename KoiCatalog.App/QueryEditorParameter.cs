using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using KoiCatalog.Data;

namespace KoiCatalog.App
{
    public sealed class QueryEditorParameter : INotifyPropertyChanged
    {
        public QueryParameterTemplate Template { get; }

        public object Value
        {
            get => _value;
            set
            {
                _value = value;
                QueryEditor.Query?.SetValue(Template, value);
                OnPropertyChanged();
            }
        }
        private object _value;

        public int SelectedItemCount => SelectableValues.Count(i => i.IsSelected);
        
        public ObservableCollection<QueryEditorParameterSelectableValue> SelectableValues { get; } =
            new ObservableCollection<QueryEditorParameterSelectableValue>();

        internal QueryEditor QueryEditor { get; }

        public QueryEditorParameter(QueryEditor queryEditor, QueryParameterTemplate template)
        {
            if (queryEditor == null) throw new ArgumentNullException(nameof(queryEditor));
            if (template == null) throw new ArgumentNullException(nameof(template));
            SelectableValues.CollectionChanged += SelectableValuesOnCollectionChanged;
            Template = template;
            QueryEditor = queryEditor;
        }

        private List<QueryEditorParameterSelectableValue> ObservedSelectableValues { get; } =
            new List<QueryEditorParameterSelectableValue>();
        
        private void StartObserving(QueryEditorParameterSelectableValue obj)
        {
            if (obj.Owner != null)
                throw new InvalidOperationException("Object already has an owner.");
            obj.PropertyChanged += SelectableValueOnPropertyChanged;
            obj.Owner = this;
            ObservedSelectableValues.Add(obj);
        }

        private void StopObserving(QueryEditorParameterSelectableValue obj)
        {
            obj.Owner = null;
            obj.PropertyChanged -= SelectableValueOnPropertyChanged;
            ObservedSelectableValues.Remove(obj);
        }

        private void SelectableValuesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.Cast<QueryEditorParameterSelectableValue>())
                    StopObserving(item);
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.Cast<QueryEditorParameterSelectableValue>())
                    StartObserving(item);
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in ObservedSelectableValues.Where(i => !SelectableValues.Contains(i)).ToList())
                    StopObserving(item);
                foreach (var item in SelectableValues.Where(i => i.Owner != this))
                    StartObserving(item);
            }
        }

        private void SelectableValueOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(QueryEditorParameterSelectableValue.IsSelected))
            {
                OnPropertyChanged(nameof(SelectedItemCount));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
