using System;
using System.Collections.Generic;

namespace KoiCatalog.Plugins.FileIO
{
    public abstract class DefaultDirectoryHandler
    {
        public virtual IEnumerable<DefaultDirectory> GetDirectories() =>
            Array.Empty<DefaultDirectory>();
    }
}
