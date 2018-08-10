using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PalettePal;

namespace KoiCatalog.Drawing
{
    public sealed class ConcurrentCachingPalette
    {
        private Palette BasePalette { get; }

        internal ConcurrentCachingPalette() { }

        public ConcurrentCachingPalette(Palette basePalette)
        {
            if (basePalette == null) throw new ArgumentNullException(nameof(basePalette));
            BasePalette = basePalette;
        }

        public IEnumerable<Adjective> GetAdjectives(Color color)
        {
            return Cache.GetOrAdd(color, k =>
            {
                lock (BasePalette)
                {
                    return BasePalette.GetAdjectives(color).ToList().AsReadOnly();
                }
            });
        }

        private ConcurrentDictionary<Color, IEnumerable<Adjective>> Cache { get; } =
            new ConcurrentDictionary<Color, IEnumerable<Adjective>>();
    }
}
