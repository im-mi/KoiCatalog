using System;
using System.Collections.Generic;
using System.Linq;

namespace KoiCatalog.Data
{
    public sealed class CompoundQueryHandler : QueryHandler
    {
        public CompoundQueryHandler(Type[] handlerTypes)
        {
            if (handlerTypes == null) throw new ArgumentNullException(nameof(handlerTypes));
            Handlers = handlerTypes
                .Select(Activator.CreateInstance)
                .Cast<QueryHandler>()
                .ToList()
                .AsReadOnly();
        }

        public override void GetQueryFormat(IReadOnlyEntity entity, QueryFormat format)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (format == null) throw new ArgumentNullException(nameof(format));

            foreach (var handler in Handlers)
            {
                handler.GetQueryFormat(entity, format);
            }
        }

        public override bool Filter(IReadOnlyEntity entity, QueryParameter param)
        {
            return Handlers.All(i => i.Filter(entity, param));
        }

        private IEnumerable<QueryHandler> Handlers { get; }
    }
}
