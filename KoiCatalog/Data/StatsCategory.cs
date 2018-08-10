using System;

namespace KoiCatalog.Data
{
    public sealed class StatsCategory : IComparable, IComparable<StatsCategory>
    {
        public string Name { get; }
        public int DisplayIndex { get; }

        public StatsCategory(string name, int displayIndex = 0)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DisplayIndex = displayIndex;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is StatsCategory))
                throw new ArgumentException();
            return CompareTo((StatsCategory)obj);
        }

        public int CompareTo(StatsCategory other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            int comparison;
            comparison = DisplayIndex.CompareTo(other.DisplayIndex);
            if (comparison != 0) return comparison;
            comparison = string.Compare(Name, other.Name, StringComparison.Ordinal);
            return comparison;
        }
    }
}
