using Demo.Core.Entities;
using Demo.Core.Specifications.Base;

namespace Demo.Core.Specifications
{
    public class ProductSpecification : BaseSpecification<Product>
    {
        public ProductSpecification(string productName) 
            : base(p => p.Name.ToLower().Contains(productName.ToLower()))
        {
            AddInclude(p => p.Category);
        }

        public ProductSpecification() : base(null)
        {
            AddInclude(p => p.Category);
        }
    }
}
