using System;
using System.Collections.Generic;

namespace KoiCatalog.Data
{
    public sealed class QueryFormatParameter
    {
        public QueryParameterTemplate Template { get; }
        public IReadOnlyList<object> SelectableValues =>
            _readOnlySelectableValues ?? (_readOnlySelectableValues = _selectableValues.AsReadOnly());
        private IReadOnlyList<object> _readOnlySelectableValues;
        private readonly List<object> _selectableValues = new List<object>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Whether a change occurred.</returns>
        internal bool TryAddSelectableValue(object value)
        {
            if (_selectableValues.Contains(value))
                return false;
            _selectableValues.Add(value);
            return true;
        }

        internal QueryFormatParameter(QueryParameterTemplate template)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
        }
    }
}
