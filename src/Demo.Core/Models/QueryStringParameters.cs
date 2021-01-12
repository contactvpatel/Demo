namespace Demo.Core.Models
{
    public class QueryStringParameters
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        
        public QueryStringParameters()
        {
            PageNumber = 1;
            PageSize = 10;
        }

        public QueryStringParameters(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 10 ? 10 : pageSize;
        }
    }
}
