using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KoiCatalog.Data
{
    /// <summary>
    /// A database connection that connects directly to a <see cref="Database"/> instance.
    /// </summary>
    public sealed class DirectDatabaseConnection : IDatabaseConnection
    {
        public event EventHandler Connected;
        public event DatabaseEventEventHandler EventOccurred;

        public Database Database
        {
            get => _database;
            set
            {
                EnsureNotConnected();
                _database = value;
            }
        }
        private Database _database;

        public bool IsConnected { get; private set; }
        
        public Task ConnectAsync()
        {
            if (Database == null) throw new InvalidOperationException($"{nameof(Database)} cannot be null.");
            EnsureNotConnected();
            IsConnected = true;
            Database.EventOccurred += DatabaseOnEventOccurred;
            Connected?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            // Todo: Cancel any in-progress operations immediately when disconnecting.
            EnsureConnected();
            IsConnected = false;
            Database.EventOccurred -= DatabaseOnEventOccurred;
            return Task.CompletedTask;
        }

        private void DatabaseOnEventOccurred(object sender, DatabaseEventEventArgs e)
        {
            EventOccurred?.Invoke(this, e);
        }
        
        public Task SaveAsync(IDatabaseSaveMethod saveMethod, CancellationToken cancellationToken = default(CancellationToken))
        {
            EnsureConnected();
            return Database.SaveAsync(saveMethod, cancellationToken);
        }

        public Task LoadAsync(IDatabaseLoadMethod loadMethod, CancellationToken cancellationToken = default(CancellationToken))
        {
            EnsureConnected();
            return Database.LoadAsync(loadMethod, cancellationToken);
        }

        public Task SubmitQueryAsync(
            Query query,
            QueryProgressDelegate queryProgressDelegate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            EnsureConnected();
            return Database.SubmitQueryAsync(query.Clone(), queryProgressDelegate, cancellationToken);
        }

        public Task RequestQueryFormatAsync(
            IEnumerable<ComponentTypeCode> componentFilter,
            RequestQueryFormatProgressDelegate requestQueryFormatProgressDelegate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            EnsureConnected();
            return Database.RequestQueryFormatAsync(
                componentFilter?.ToArray() ?? Array.Empty<ComponentTypeCode>(),
                requestQueryFormatProgressDelegate,
                cancellationToken);
        }

        private void EnsureConnected()
        {
            if (!IsConnected)
                throw new InvalidOperationException("A connection has not been made.");
        }

        private void EnsureNotConnected()
        {
            if (IsConnected)
                throw new InvalidOperationException("A connection is already in progress or has already been made.");
        }
    }
}
