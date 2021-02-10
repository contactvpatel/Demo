using System.ComponentModel.DataAnnotations;

namespace Demo.Api.Dto
{
    public class CategoryUpdateRequest
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        public string Name { get; set; }

        public string Description { get; set; }

        public int LastUpdatedBy { get; set; }
    }
}