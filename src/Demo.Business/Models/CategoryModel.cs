using Demo.Business.Models.Base;

namespace Demo.Business.Models
{
    public class CategoryModel : BaseModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
