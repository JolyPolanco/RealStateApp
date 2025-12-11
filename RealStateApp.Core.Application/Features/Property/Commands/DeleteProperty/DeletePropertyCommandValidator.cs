using FluentValidation;

namespace RealStateApp.Core.Application.Features.Property.Commands.DeleteProperty
{
    public class DeletePropertyCommandValidator : AbstractValidator<DeletePropertyCommand>
    {
        public DeletePropertyCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotNull().WithMessage("Code is required")
                .NotEmpty().WithMessage("Code is required")
                .Length(6, 6).WithMessage("Code must be 6 characters long");
        }
    }
}
