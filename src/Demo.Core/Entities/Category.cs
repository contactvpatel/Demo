using Demo.Core.Entities.Base;

namespace Demo.Core.Entities
{
    public partial class Category : Entity
    {
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }


        public virtual ICollection<Product> Products { get; } = new List<Product>();

        public static Category Create(int categoryId, string name, string description = null)
        {
            var category = new Category
            {
                CategoryId = categoryId,
                Name = name,
                Description = description
            };
            return category;
        }
    }
}