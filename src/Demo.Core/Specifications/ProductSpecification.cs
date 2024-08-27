using Demo.Core.Entities;
using Demo.Core.Specifications.Base;

namespace Demo.Core.Specifications
{
    public sealed class ProductSpecification : BaseSpecification<Product>
    {
        public ProductSpecification() : base(p => !p.IsDeleted)
        {
            //AddInclude(p => p.Category);
        }

        public ProductSpecification(string productName)
            : base(p => !p.IsDeleted && p.Name.ToLower().Contains(productName.ToLower()))
        {
            //AddInclude(p => p.Category);
        }
    }
}
