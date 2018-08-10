using System;

namespace KoiCatalog.Data
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DisplayAttribute : Attribute
    {
        public static DisplayAttribute Default { get; } = new DisplayAttribute();

        public string Name { get; }
        public int Order { get; }
        public Visibility Visibility { get; }
        public double Width { get; }
        public double MinWidth { get; }

        public DisplayAttribute(
            string name = null,
            int order = 0,
            Visibility visibility = Visibility.Visible,
            double width = 100,
            double minWidth = 20)
        {
            Name = name;
            Order = order;
            Visibility = visibility;
            Width = width;
            MinWidth = minWidth;
        }
    }
}
