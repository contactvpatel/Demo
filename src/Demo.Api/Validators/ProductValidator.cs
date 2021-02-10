using Demo.Api.Dto;
using FluentValidation;

namespace Demo.Api.Validators
{
    public class ProductCreateValidator : AbstractValidator<ProductCreateRequest>
    {
        public ProductCreateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty() // Shouldn't be empty
                .Matches("^[a-zA-Z0-9 ]*$"); //Any Alphanumeric Value
        }
    }

    public class ProductUpdateValidator : AbstractValidator<ProductUpdateRequest>
    {
        public ProductUpdateValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty() // Shouldn't be empty
                .Matches("^[a-zA-Z0-9 ]*$"); //Any Alphanumeric Value
        }
    }
}
