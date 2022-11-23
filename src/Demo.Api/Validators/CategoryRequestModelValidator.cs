using Demo.Api.Dto;
using Demo.Api.Models;
using FluentValidation;

namespace Demo.Api.Validators
{
    public class CategoryRequestModelValidator : AbstractValidator<CategoryRequestModel>
    {
        public CategoryRequestModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category Name is required.") // Shouldn't be null or empty
                .Matches("^[a-zA-Z0-9 ]*$").WithMessage("Category Name must be alphanumeric."); //Any Alphanumeric Value
        }
    }
}
