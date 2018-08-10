using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KoiCatalog.Data
{
    public delegate void QueryProgressDelegate(QueryResult result);
    public delegate void RequestQueryFormatProgressDelegate(QueryFormat.Action action);

    public delegate void DatabaseEventEventHandler(object sender, DatabaseEventEventArgs e);

    public interface IDatabaseConnection
    {
        Task ConnectAsync();
        Task DisconnectAsync();
        event EventHandler Connected;
        bool IsConnected { get; }

        event DatabaseEventEventHandler EventOccurred;

        Task LoadAsync(IDatabaseLoadMethod loadMethod,
            CancellationToken cancellationToken = default(CancellationToken));
        Task SaveAsync(IDatabaseSaveMethod saveMethod,
            CancellationToken cancellationToken = default(CancellationToken));
        Task SubmitQueryAsync(Query query,
            QueryProgressDelegate queryProgressDelegate,
            CancellationToken cancellationToken = default(CancellationToken));
        Task RequestQueryFormatAsync(
            IEnumerable<ComponentTypeCode> componentFilter,
            RequestQueryFormatProgressDelegate requestQueryFormatProgressDelegate,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
