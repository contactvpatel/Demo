namespace Demo.Api.Models
{
    public class CategoryApiModel : BaseApiModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
