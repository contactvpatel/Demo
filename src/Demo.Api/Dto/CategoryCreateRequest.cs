using System.ComponentModel.DataAnnotations;

namespace Demo.Api.Dto
{
    public class CategoryCreateRequest
    {
        [Required(ErrorMessage = "Category Name is required")]
        public string Name { get; set; }

        public string Description { get; set; }

        public int CreatedBy { get; set; }
    }
}