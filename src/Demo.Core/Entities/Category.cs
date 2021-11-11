using Demo.Core.Entities.Base;

namespace Demo.Core.Entities
{
    public class Category : Entity
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Product> Products { get; private set; }

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
