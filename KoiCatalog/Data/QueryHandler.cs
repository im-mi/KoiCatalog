namespace KoiCatalog.Data
{
    public abstract class QueryHandler
    {
        public virtual void GetQueryFormat(IReadOnlyEntity entity, QueryFormat format) { }
        public virtual bool Filter(IReadOnlyEntity entity, QueryParameter param) => true;
    }
}
