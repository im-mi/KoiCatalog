namespace KoiCatalog.Data
{
    public sealed class QueryResult
    {
        public IReadOnlyEntity Data { get; }

        public QueryResult(IReadOnlyEntity data)
        {
            Data = data;
        }
    }
}
