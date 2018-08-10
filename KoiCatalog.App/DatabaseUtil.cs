using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using KoiCatalog.Data;
using KoiCatalog.Plugins.FileIO;

namespace KoiCatalog.App
{
    public static class DatabaseUtil
    {
        /// <summary>
        /// A high-level database save/load method that handles caching automatically.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="directory"></param>
        /// <param name="cacheDirectory"></param>
        /// <param name="mode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task LoadAsync(
            this IDatabaseConnection connection,
            string directory,
            string cacheDirectory,
            DatabaseLoadMode mode = DatabaseLoadMode.Auto,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (directory == null) throw new ArgumentNullException(nameof(directory));
            if (cacheDirectory == null) throw new ArgumentNullException(nameof(cacheDirectory));

            return Task.Run(async () =>
            {
                var cacheFilePath = Path.Combine(cacheDirectory, IO.IOUtil.PathToHash(directory));

                IDatabaseLoadMethod loadMethod = null;
                IDatabaseSaveMethod saveMethod = null;

                try
                {
                    switch (mode)
                    {
                        case DatabaseLoadMode.Auto:
                            if (File.Exists(cacheFilePath))
                                goto case DatabaseLoadMode.LoadCache;
                            else
                                goto case DatabaseLoadMode.Rebuild;
                        case DatabaseLoadMode.LoadCache:
                            loadMethod = new SerializedDatabaseLoadMethod(cacheFilePath);
                            break;
                        case DatabaseLoadMode.Rebuild:
                            loadMethod = new CreateFromDirectoryDatabaseLoadMethod(directory, FileIOUtil.OmniFileHandler);
                            saveMethod = new SerializedDatabaseSaveMethod(cacheFilePath);
                            break;
                        case DatabaseLoadMode.Refresh:
                            loadMethod = new CreateFromDirectoryDatabaseLoadMethod(directory, FileIOUtil.OmniFileHandler, update: true);
                            saveMethod = new SerializedDatabaseSaveMethod(cacheFilePath);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                    }

                    await connection.LoadAsync(loadMethod, cancellationToken);

                    if (saveMethod != null)
                    {
                        await connection.SaveAsync(saveMethod, cancellationToken);
                    }
                }
                catch (TaskCanceledException)
                {
                }
                finally
                {
                    loadMethod?.Dispose();
                    saveMethod?.Dispose();
                }
            }, cancellationToken);
        }
    }
}
