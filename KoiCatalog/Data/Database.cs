using KoiCatalog.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KoiCatalog.Data
{
    public sealed class Database
    {
        private volatile ConcurrentList<Entity> volatileDatabaseItems = new ConcurrentList<Entity>();
        private readonly object DbWriteLock = new object();

        private const int ProgressEventDelay = 500;

        internal event DatabaseEventEventHandler EventOccurred;

        private void Log(DatabaseEvent e)
        {
            EventOccurred?.Invoke(this, new DatabaseEventEventArgs(e));
        }

        private class ProxyLogger : ILogger
        {
            private Database Owner { get; }

            public ProxyLogger(Database owner)
            {
                Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            public void Log(DatabaseEvent message)
            {
                Owner.Log(message);
            }
        }

        private ProxyLogger Logger { get; }
        private QueryHandler QueryHandler { get; }

        public Database(QueryHandler queryHandler)
        {
            if (queryHandler == null) throw new ArgumentNullException(nameof(queryHandler));
            QueryHandler = queryHandler;
            Logger = new ProxyLogger(this);
            volatileDatabaseItems.CompleteAdding();
        }

        // Todo: Cancel or wait for any unfinished operations (queries, etc.) when reloading the database.

        internal Task LoadAsync(IDatabaseLoadMethod loadMethod,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (loadMethod == null) throw new ArgumentNullException(nameof(loadMethod));

            return Task.Run(() =>
            {
                if (cancellationToken.IsCancellationRequested) return;

                lock (DbWriteLock)
                {
                    var databaseItems = new ConcurrentList<Entity>();
                    List<Entity> previousItems;
                    using (var e = volatileDatabaseItems.GetEnumerator(cancellationToken))
                    {
                        previousItems = e.AsEnumerable().ToList();
                    }
                    if (cancellationToken.IsCancellationRequested) return;

                    volatileDatabaseItems = databaseItems;

                    try
                    {
                        Log(new DatabaseEvent(DatabaseEventType.LoadStart, message: "Database load started."));

                        if (loadMethod.Scannable)
                        {
                            Log(new DatabaseEvent(DatabaseEventType.ScanStart, message: "Scan started."));
                            loadMethod.Scan(previousItems, Logger, cancellationToken);
                            if (cancellationToken.IsCancellationRequested) return;
                            foreach (var item in previousItems)
                                databaseItems.Add(item);
                            Log(new DatabaseEvent(DatabaseEventType.ScanEnd, message: "Scan ended."));

                            Log(new DatabaseEvent(DatabaseEventType.LoadProgress,
                                message: $"Load progress: 0 / {loadMethod.ItemCount:n0}",
                                progress: new Progress(0, loadMethod.ItemCount)));
                        }

                        var items = loadMethod.Load(Logger);

                        var stopwatch = new Stopwatch();
                        stopwatch.Start();
                        var i = -1;
                        foreach (var item in items)
                        {
                            i++;
                            if (cancellationToken.IsCancellationRequested) break;

                            if (item != null)
                            {
                                databaseItems.Add(item);
                            }

                            if (loadMethod.Scannable)
                            {
                                if (stopwatch.ElapsedMilliseconds >= ProgressEventDelay)
                                {
                                    Log(new DatabaseEvent(DatabaseEventType.LoadProgress,
                                        progress: new Progress(i + 1, loadMethod.ItemCount)));
                                    stopwatch.Restart();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log(new DatabaseEvent(
                            DatabaseEventType.Message, message: $"Database load error: {ex.Message}",
                            severity: EventSeverity.Error));
                    }
                    finally
                    {
                        databaseItems.CompleteAdding();
                        Log(new DatabaseEvent(DatabaseEventType.LoadEnd, message: "Database load ended."));
                    }
                }
            }, cancellationToken);
        }

        internal Task SaveAsync(IDatabaseSaveMethod saveMethod,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (saveMethod == null) throw new ArgumentNullException(nameof(saveMethod));

            return Task.Run(() =>
            {
                if (cancellationToken.IsCancellationRequested) return;

                try
                {
                    Log(new DatabaseEvent(DatabaseEventType.SaveStart, message: "Database save started."));

                    List<Entity> items;
                    using (var e = volatileDatabaseItems.GetEnumerator(cancellationToken))
                    {
                        items = e.AsEnumerable().ToList();
                    }
                    if (cancellationToken.IsCancellationRequested) return;

                    try
                    {
                        saveMethod.Save(items, Logger);
                    }
                    catch (Exception ex)
                    {
                        Log(new DatabaseEvent(DatabaseEventType.Message,
                            message: $"Database save error: {ex.Message}",
                            severity: EventSeverity.Error));
                    }
                }
                finally
                {
                    Log(new DatabaseEvent(DatabaseEventType.SaveEnd, message: "Database save ended."));
                }
            }, cancellationToken);
        }

        internal Task SubmitQueryAsync(Query query,
            QueryProgressDelegate queryProgressDelegate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            return Task.Run(() =>
            {
                if (cancellationToken.IsCancellationRequested) return;
                
                var componentFilter = query.ComponentFilter.Count > 0
                    ? (IReadOnlyComponentFilter)new ReadOnlyComponentFilter(query.ComponentFilter)
                    : FullAccessComponentFilter.Instance;

                using (var e = volatileDatabaseItems.GetEnumerator(cancellationToken))
                {
                    foreach (var entity in e.AsEnumerable().Where(i => i.HasComponents(query.ComponentFilter)))
                    {
                        if (cancellationToken.IsCancellationRequested) return;
                        var readOnlyEntity = entity.AsReadOnly(componentFilter);
                        if (!query.Filter(readOnlyEntity, QueryHandler)) continue;
                        queryProgressDelegate?.Invoke(new QueryResult(readOnlyEntity));
                    }
                }
            }, cancellationToken);
        }

        internal Task RequestQueryFormatAsync(
            ComponentTypeCode[] componentFilter,
            RequestQueryFormatProgressDelegate requestQueryFormatProgressDelegate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (componentFilter == null) throw new ArgumentNullException(nameof(componentFilter));

            return Task.Run(() =>
            {
                if (cancellationToken.IsCancellationRequested) return;
                
                var format = new QueryFormat();
                format.Changed += (sender, e) =>
                {
                    requestQueryFormatProgressDelegate?.Invoke(e.Action);
                };

                var componentFilter2 = new ReadOnlyComponentFilter(componentFilter);

                using (var e = volatileDatabaseItems.GetEnumerator(cancellationToken))
                {
                    foreach (var entity in e.AsEnumerable().Where(i => i.HasComponents(componentFilter)))
                    {
                        if (cancellationToken.IsCancellationRequested) return;
                        var readOnlyEntity = entity.AsReadOnly(componentFilter2);
                        QueryHandler.GetQueryFormat(readOnlyEntity, format);
                    }
                }
            }, cancellationToken);
        }
    }
}
