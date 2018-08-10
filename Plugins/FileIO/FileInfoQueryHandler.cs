using KoiCatalog.Data;

namespace KoiCatalog.Plugins.FileIO
{
    public sealed class FileInfoQueryHandler : QueryHandler
    {
        public static class Categories
        {
            public static readonly QueryParameterTemplateCategory FileInfo =
                new QueryParameterTemplateCategory("File Info", displayIndex: 1000);
        }

        public static class Parameters
        {
            public static readonly QueryParameterTemplate FileName =
                new QueryParameterTemplate("File Name", typeof(string), Categories.FileInfo, displayIndex: 0);
            public static readonly QueryParameterTemplate HashMD5 =
                new QueryParameterTemplate("MD5", typeof(string), Categories.FileInfo, displayIndex: 1);
            public static readonly QueryParameterTemplate HashSHA1OfMD5 =
                new QueryParameterTemplate("SHA-1 of MD5", typeof(string), Categories.FileInfo, displayIndex: 2);
        }

        public override void GetQueryFormat(IReadOnlyEntity entity, QueryFormat format)
        {
            var fileInfo = entity.GetComponent(FileInfo.TypeCode);
            if (fileInfo == null) return;

            format.AddParameter(Parameters.FileName);
            format.AddParameter(Parameters.HashMD5);
            format.AddParameter(Parameters.HashSHA1OfMD5);
        }

        public override bool Filter(IReadOnlyEntity entity, QueryParameter param)
        {
            var fileInfo = entity.GetComponent(FileInfo.TypeCode);
            if (fileInfo == null) return true;

            if (param.Template == Parameters.FileName)
                return param.FilterOperatorString(fileInfo.FileName);
            else if (param.Template == Parameters.HashMD5)
                return param.FilterOperatorString(fileInfo.HashMD5);
            else if (param.Template == Parameters.HashSHA1OfMD5)
                return param.FilterOperatorString(fileInfo.HashSHA1OfMD5);
            else
                return true;
        }
    }
}
