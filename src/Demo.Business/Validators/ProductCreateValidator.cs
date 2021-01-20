using Demo.Business.Models;
using FluentValidation;

namespace Demo.Business.Validators
{
    public class ProductCreateValidator : AbstractValidator<ProductModel>
    {
        public ProductCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty() // Shouldn't be empty
                .Matches("^[a-zA-Z0-9 ]*$"); //Any Alphanumeric Value
        }
    }
}
