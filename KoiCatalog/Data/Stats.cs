using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace KoiCatalog.Data
{
    public sealed class Stats : ObservableCollection<Stat>
    {
        /// <summary>
        /// Creats a stat.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="category"></param>
        /// <returns>The stat that was created, or the existing stat with the same key and category, if applicable.</returns>
        public Stat Create(object key, StatsCategory category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            var stat = this.FirstOrDefault(i => Equals(i.Key, key) && Equals(i.Category, category));
            if (stat == null)
            {
                stat = new Stat(key, 0, category);
                Add(stat);
            }
            return stat;
        }

        public void Increment(object name, StatsCategory category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            var stat = Create(name, category);
            stat.Value++;
        }
    }
}
