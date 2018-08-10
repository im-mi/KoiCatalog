using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using KoiCatalog.Data;

namespace KoiCatalog.Plugins.FileIO
{
    public sealed class CreateFromDirectoryDatabaseLoadMethod : DatabaseLoadMethod
    {
        public string Directory
        {
            get
            {
                CheckDisposed();
                return _directory;
            }
        }
        private readonly string _directory;

        public bool UpdateEnabled
        {
            get
            {
                CheckDisposed();
                return _update;
            }
        }
        private readonly bool _update;

        public FileHandler FileHandler
        {
            get
            {
                CheckDisposed();
                return _fileHandler;
            }
        }
        private FileHandler _fileHandler;

        public CreateFromDirectoryDatabaseLoadMethod(string directory, FileHandler fileHandler, bool update = false)
        {
            _directory = directory ?? throw new ArgumentNullException(nameof(directory));
            _fileHandler = fileHandler ?? throw new ArgumentNullException(nameof(fileHandler));
            _update = update;
        }
        
        private readonly List<Uri> filesToLoad = new List<Uri>();

        public override bool Scannable => true;

        protected override int ScanInternal(List<Entity> currentItems, ILogger logger, CancellationToken cancellationToken)
        {
            var missingItemCount = 0;
            var unindexedOrModifiedItemCount = 0;

            if (UpdateEnabled)
            {
                var itemsToKeep = new List<Entity>();

                foreach (var i in currentItems)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return 0;

                    var fileInfo = i.GetComponent(FileInfo.TypeCode);
                    if (fileInfo == null) continue;

                    try
                    {
                        if (!File.Exists(fileInfo.File.LocalPath))
                        {
                            missingItemCount++;
                            continue;
                        }

                        if (File.GetLastWriteTimeUtc(fileInfo.File.LocalPath) != fileInfo.ModifiedTime)
                        {
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Log(new DatabaseEvent(
                            DatabaseEventType.Message, message: $"Scan error: {ex}", severity: EventSeverity.Error));
                    }

                    itemsToKeep.Add(i);
                }

                currentItems.Clear();
                currentItems.AddRange(itemsToKeep);
            }
            else
            {
                currentItems.Clear();
            }

            var unchangedItemsUris = currentItems.Select(i => i.GetComponent(FileInfo.TypeCode)).ToLookup(i => i.File);
            var files = System.IO.Directory.GetFiles(Directory);
            foreach (var file in files)
            {
                var uri = new Uri(Path.Combine(Directory, file));
                if (unchangedItemsUris.Contains(uri))
                    continue;
                filesToLoad.Add(uri);
                unindexedOrModifiedItemCount++;
            }

            logger.Log(new DatabaseEvent(DatabaseEventType.Message,
                message: $"Scan found {missingItemCount:n0} missing {(missingItemCount == 1 ? "item" : "items")}."));
            logger.Log(new DatabaseEvent(DatabaseEventType.Message,
                message: $"Scan found {unindexedOrModifiedItemCount:n0} unindexed or modified {(unindexedOrModifiedItemCount == 1 ? "item" : "items")}."));

            return filesToLoad.Count;
        }

        protected override IEnumerable<Entity> LoadInternal(ILogger logger)
        {
            var loader = new FileLoader();

            foreach (var uri in filesToLoad)
            {
                var entity = new Entity();
                loader.Entity = entity;
                loader.Source = uri;
                loader.Logger = logger;

                try
                {
                    FileHandler.HandleFile(loader);
                }
                catch (Exception ex)
                {
                    loader.LogError(ex.Message);
                    entity = null;
                }

                try
                {
                    FileHandler.FinishHandleFile(loader);
                }
                catch (Exception ex)
                {
                    loader.LogError(ex.Message);
                    entity = null;
                }

                yield return entity;
            }
        }
    }
}
