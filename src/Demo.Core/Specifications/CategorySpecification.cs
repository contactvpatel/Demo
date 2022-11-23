using Demo.Core.Entities;
using Demo.Core.Specifications.Base;

namespace Demo.Core.Specifications
{
    public sealed class CategorySpecification : BaseSpecification<Category>
    {
        public CategorySpecification()
            : base(b => !b.IsDeleted)
        {
            AddInclude(b => b.Products);
        }

        public CategorySpecification(int categoryId)
            : base(b => !b.IsDeleted && b.CategoryId == categoryId)
        {
            AddInclude(b => b.Products);
        }
    }
}