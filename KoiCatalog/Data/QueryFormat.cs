using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KoiCatalog.Data
{
    public sealed class QueryFormat : IEnumerable<QueryFormatParameter>, INotifyPropertyChanged
    {
        public delegate void ChangedEventHandler(object sender, ChangedEventArgs e);
        public event ChangedEventHandler Changed;
        public event PropertyChangedEventHandler PropertyChanged;

        private List<QueryFormatParameter> Parameters { get; } = new List<QueryFormatParameter>();

        public int ParameterCount => Parameters.Count;

        public IEnumerator<QueryFormatParameter> GetEnumerator() => Parameters.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void AddParameter(QueryParameterTemplate template) =>
            PerformAction(Action.CreateAddParameterAction(template));
        public void AddSelectableValue(QueryParameterTemplate template, object selectableValue) =>
            PerformAction(Action.CreateAddSelectableValueAction(template, selectableValue));
        public void Clear() =>
            PerformAction(Action.CreateClearAction());

        public void PerformAction(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (PerformActionInternal(action))
            {
                Changed?.Invoke(this, new ChangedEventArgs(action));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns>Whether a change occurred.</returns>
        private bool PerformActionInternal(Action action)
        {
            switch (action.ActionType)
            {
                case Action.QueryFormatActionType.AddParameter:
                    {
                        return CreateParameter(action.Template, out var _);
                    }
                case Action.QueryFormatActionType.AddSelectableValue:
                    {
                        CreateParameter(action.Template, out var parameter);
                        return parameter.TryAddSelectableValue(action.NewSelectableValue);
                    }
                case Action.QueryFormatActionType.Clear:
                    {
                        if (Parameters.Count > 0)
                        {
                            Parameters.Clear();
                            OnPropertyChanged(nameof(ParameterCount));
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="template"></param>
        /// <returns>Whether a change occurred.</returns>
        private bool CreateParameter(QueryParameterTemplate template, out QueryFormatParameter parameter)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            foreach (var existingParameter in Parameters)
            {
                if (Equals(existingParameter.Template, template))
                {
                    parameter = existingParameter;
                    return false;
                }
            }
            parameter = new QueryFormatParameter(template);
            Parameters.Add(parameter);
            OnPropertyChanged(nameof(ParameterCount));
            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public sealed class Action
        {
            public enum QueryFormatActionType
            {
                AddParameter,
                AddSelectableValue,
                Clear,
            }

            public QueryFormatActionType ActionType { get; }
            public QueryParameterTemplate Template { get; private set; }
            public object NewSelectableValue { get; private set; }

            private Action(QueryFormatActionType actionType)
            {
                ActionType = actionType;
            }

            public static Action CreateAddParameterAction(QueryParameterTemplate template)
            {
                if (template == null) throw new ArgumentNullException(nameof(template));
                return new Action(QueryFormatActionType.AddParameter)
                {
                    Template = template,
                };
            }

            public static Action CreateAddSelectableValueAction(
                QueryParameterTemplate template, object selectableValue)
            {
                if (template == null) throw new ArgumentNullException(nameof(template));
                return new Action(QueryFormatActionType.AddSelectableValue)
                {
                    Template = template,
                    NewSelectableValue = selectableValue,
                };
            }

            public static Action CreateClearAction()
            {
                return new Action(QueryFormatActionType.Clear);
            }
        }

        public sealed class ChangedEventArgs : EventArgs
        {
            public Action Action { get; }

            public ChangedEventArgs(Action action)
            {
                if (action == null) throw new ArgumentNullException(nameof(action));
                Action = action;
            }
        }
    }
}
