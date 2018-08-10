using System;
using System.Collections.Generic;
using System.Linq;

namespace KoiCatalog.Plugins.FileIO
{
    public sealed class CompoundDefaultDirectoryHandler : DefaultDirectoryHandler
    {
        public CompoundDefaultDirectoryHandler(Type[] handlerTypes)
        {
            if (handlerTypes == null) throw new ArgumentNullException(nameof(handlerTypes));
            Handlers = handlerTypes
                .Select(Activator.CreateInstance)
                .Cast<DefaultDirectoryHandler>()
                .ToList()
                .AsReadOnly();
        }

        public override IEnumerable<DefaultDirectory> GetDirectories()
        {
            var results = new List<DefaultDirectory>();
            foreach (var fileHandler in Handlers)
            {
                results.AddRange(fileHandler.GetDirectories());
            }
            return results;
        }

        private IEnumerable<DefaultDirectoryHandler> Handlers { get; }
    }
}
