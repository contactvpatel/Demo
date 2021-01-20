using Demo.Api.Models;
using FluentValidation;

namespace Demo.Api.Validators
{
    public class ProductCreateValidator : AbstractValidator<ProductApiModel>
    {
        public ProductCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty() // Shouldn't be empty
                .Matches("^[a-zA-Z0-9 ]*$"); //Any Alphanumeric Value
        }
    }
}
