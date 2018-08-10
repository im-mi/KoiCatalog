using System;

namespace KoiCatalog.Util
{
    public interface ICloneable<T> : ICloneable
    {
        new T Clone();
    }
}
