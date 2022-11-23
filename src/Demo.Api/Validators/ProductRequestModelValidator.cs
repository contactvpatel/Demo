using Demo.Api.Dto;
using FluentValidation;

namespace Demo.Api.Validators
{
    public class ProductRequestModelValidator : AbstractValidator<ProductRequestModel>
    {
        public ProductRequestModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product Name is required.") // Shouldn't be null or empty
                .Matches("^[a-zA-Z0-9 ]*$").WithMessage("Product Name must be alphanumeric."); //Any Alphanumeric Value
        }
    }
}
