using FluentValidation;

namespace RealStateApp.Core.Application.Features.Property.Commands.UpdateProperty
{
    public class UpdatePropertyCommandValidator : AbstractValidator<UpdatePropertyCommand>
    {
        public UpdatePropertyCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es requerido")
                .NotNull().WithMessage("El código es requerido")
                .Length(6, 6).WithMessage("El código debe tener 6 caracteres");

            RuleFor(x => x.Price)
                .NotNull().WithMessage("El precio es requerido")
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");

            RuleFor(x => x.SizeInMeters)
                .NotNull().WithMessage("El tamaño en metros es requerido")
                .GreaterThan(0).WithMessage("El tamaño debe ser mayor a 0");

            RuleFor(x => x.Bedrooms)
                .NotNull().WithMessage("La cantidad de habitaciones es requerida")
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad de habitaciones debe ser mayor o igual a 0");

            RuleFor(x => x.Bathrooms)
                .NotNull().WithMessage("La cantidad de baños es requerida")
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad de baños debe ser mayor o igual a 0");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es requerida")
                .NotNull().WithMessage("La descripción es requerida");

            RuleFor(x => x.PropertyTypeId)
                .NotNull().WithMessage("El tipo de propiedad es requerido")
                .GreaterThan(0).WithMessage("El tipo de propiedad debe ser válido");

            RuleFor(x => x.SaleTypeId)
                .NotNull().WithMessage("El tipo de venta es requerido")
                .GreaterThan(0).WithMessage("El tipo de venta debe ser válido");

            RuleFor(x => x.Status)
                .NotNull().WithMessage("El estado es requerido")
                .IsInEnum().WithMessage("El estado debe ser un valor válido");
        }
    }
}
