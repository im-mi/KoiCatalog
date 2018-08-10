using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KoiCatalog.Data;

namespace KoiCatalog.App
{
    /// <summary>
    /// Assists in building a <see cref="QueryEditor"/> based on a <see cref="QueryFormat"/>.
    /// </summary>
    public sealed class QueryFormatToQueryEditorService : INotifyPropertyChanged
    {
        public QueryFormat QueryFormat
        {
            get => _queryFormat;
            set
            {
                if (value == _queryFormat) return;

                if (_queryFormat != null)
                {
                    _queryFormat.Changed -= QueryFormatOnChanged;
                }

                _queryFormat = value;

                if (_queryFormat != null)
                {
                    _queryFormat.Changed += QueryFormatOnChanged;
                }

                Rebuild();
                OnPropertyChanged();
            }
        }
        private QueryFormat _queryFormat;

        public QueryEditor QueryEditor
        {
            get => _queryEditor;
            set
            {
                if (value == _queryEditor) return;
                _queryEditor = value;
                Rebuild();
                OnPropertyChanged();
            }
        }
        private QueryEditor _queryEditor = new QueryEditor();

        public QueryFormatToQueryEditorService()
        {
            QueryFormat = new QueryFormat();
        }

        public void Rebuild()
        {
            if (QueryEditor == null) return;

            QueryEditor.Clear();

            foreach (var parameter in QueryFormat)
            {
                var parameter2 = QueryEditor.CreateParameter(parameter.Template);
                foreach (var selectableValue in parameter.SelectableValues)
                {
                    var formattedSelectableValue = new QueryEditorParameterSelectableValue
                    {
                        Value = selectableValue
                    };
                    parameter2.SelectableValues.Add(formattedSelectableValue);
                }
            }
        }

        private void QueryFormatOnChanged(object sender, QueryFormat.ChangedEventArgs e)
        {
            if (QueryEditor == null) return;

            switch (e.Action.ActionType)
            {
                case QueryFormat.Action.QueryFormatActionType.AddParameter:
                    {
                        QueryEditor.CreateParameter(e.Action.Template);
                        break;
                    }
                case QueryFormat.Action.QueryFormatActionType.AddSelectableValue:
                    {
                        var parameter = QueryEditor.CreateParameter(e.Action.Template);
                        var selectableValue = new QueryEditorParameterSelectableValue();
                        selectableValue.Value = e.Action.NewSelectableValue;
                        parameter.SelectableValues.Add(selectableValue);
                        break;
                    }
                case QueryFormat.Action.QueryFormatActionType.Clear:
                    {
                        QueryEditor.Clear();
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
