using Demo.Core.Entities;
using Demo.Core.Specifications.Base;

namespace Demo.Core.Specifications
{
    public sealed class CategorySpecification : BaseSpecification<Category>
    {
        public CategorySpecification(int categoryId)
            : base(b => b.CategoryId == categoryId)
        {
            AddInclude(b => b.Products);
        }
    }
}