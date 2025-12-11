using FluentValidation;

namespace RealStateApp.Core.Application.Features.Property.Commands.UpdateProperty
{
    public class UpdatePropertyCommandValidator : AbstractValidator<UpdatePropertyCommand>
    {
        public UpdatePropertyCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required")
                .NotNull().WithMessage("Code is required")
                .Length(6, 6).WithMessage("Code must be 6 characters long");

            RuleFor(x => x.Price)
                .NotNull().WithMessage("Price is required")
                .GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.SizeInMeters)
                .NotNull().WithMessage("Size in meters is required")
                .GreaterThan(0).WithMessage("Size must be greater than 0");

            RuleFor(x => x.Bedrooms)
                .NotNull().WithMessage("Number of bedrooms is required")
                .GreaterThanOrEqualTo(0).WithMessage("Number of bedrooms must be greater than or equal to 0");

            RuleFor(x => x.Bathrooms)
                .NotNull().WithMessage("Number of bathrooms is required")
                .GreaterThanOrEqualTo(0).WithMessage("Number of bathrooms must be greater than or equal to 0");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .NotNull().WithMessage("Description is required");

            RuleFor(x => x.PropertyTypeId)
                .NotNull().WithMessage("Property type is required")
                .GreaterThan(0).WithMessage("Property type must be valid");

            RuleFor(x => x.SaleTypeId)
                .NotNull().WithMessage("Sale type is required")
                .GreaterThan(0).WithMessage("Sale type must be valid");

            RuleFor(x => x.Status)
                .NotNull().WithMessage("Status is required")
                .IsInEnum().WithMessage("Status must be a valid value");
        }
    }
}
