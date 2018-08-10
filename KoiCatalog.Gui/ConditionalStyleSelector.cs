using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KoiCatalog.Gui
{
    public abstract class StyleTrigger
    {
        public Style Style { get; set; }
        public abstract bool CheckObject(object obj);
    }

    public sealed class TypeStyleTrigger : StyleTrigger
    {
        public Type Type { get; set; }

        public override bool CheckObject(object obj)
        {
            return Type != null && Type.IsInstanceOfType(obj);
        }
    }

    public sealed class StyleTriggerCollection : Collection<StyleTrigger> { }

    sealed class ConditionalStyleSelector : StyleSelector
    {
        public StyleTriggerCollection Triggers { get; } =
            new StyleTriggerCollection();

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var trigger = Triggers?.FirstOrDefault(i => i.CheckObject(item));
            if (trigger != null)
                return trigger.Style;
            return base.SelectStyle(item, container);
        }
    }
}
