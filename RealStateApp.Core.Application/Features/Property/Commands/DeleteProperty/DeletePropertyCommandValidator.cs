using FluentValidation;

namespace RealStateApp.Core.Application.Features.Property.Commands.DeleteProperty
{
    public class DeletePropertyCommandValidator : AbstractValidator<DeletePropertyCommand>
    {
        public DeletePropertyCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es requerido")
                .NotNull().WithMessage("El código es requerido")
                .Length(6, 6).WithMessage("El código debe tener 6 caracteres");
        }
    }
}
