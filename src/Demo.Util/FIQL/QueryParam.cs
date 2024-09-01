namespace Demo.Util.FIQL
{
    public class QueryParam
    {
        public string Fields { get; set; }
        public string Filters { get; set; }
        public string Include { get; set; }
        public string Sort { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class SubQueryParam
    {
        public string ObjectName { get; set; }
        public string Fields { get; set; }
        public string Filters { get; set; }
        public string Include { get; set; }
    }
}