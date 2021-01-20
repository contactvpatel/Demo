namespace Demo.Core.Models
{
    public class PaginationQuery
    {
        public int PageNumber { get; set; }
        
        public int PageSize { get; set; }
        
        public bool IncludeTotalCount { get; set; }

        public PaginationQuery()
        {
            PageNumber = 1;
            PageSize = 10;
        }

        public PaginationQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 10 ? 10 : pageSize;
        }
    }
}
