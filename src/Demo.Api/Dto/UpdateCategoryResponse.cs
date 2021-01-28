namespace Demo.Api.Dto
{
    public class UpdateCategoryResponse
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int LastUpdatedBy { get; set; }
    }
}