using System;
using System.Collections.Generic;

namespace KoiCatalog.Util
{
    static class EnumeratorExtension
    {
        public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> enumerator)
        {
            if (enumerator == null) throw new ArgumentNullException(nameof(enumerator));
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }
    }
}
