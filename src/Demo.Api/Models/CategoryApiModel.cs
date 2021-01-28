using System.ComponentModel.DataAnnotations;

namespace Demo.Api.Models
{
    public class CategoryApiModel : BaseApiModel
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
