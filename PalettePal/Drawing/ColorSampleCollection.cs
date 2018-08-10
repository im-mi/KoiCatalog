using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace PalettePal
{
    public sealed class ColorSampleCollection : ObservableCollection<ColorSample>
    {
        public override string ToString()
        {
            var sb = new StringBuilder();
            var i = -1;
            foreach (var sample in this)
            {
                i++;
                if (i != 0)
                {
                    sb.Append(" ");
                }
                sb.Append(sample);
            }

            return sb.ToString();
        }

        public static IEnumerable<ColorSample> Parse(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            foreach (var item in value.Split().Where(i => i.Length > 0))
            {
                var sample = ColorSample.Parse(item);
                yield return sample;
            }
        }
    }
}
