using FluentValidation;


namespace RealStateApp.Core.Application.Features.SaleType.Commands.DeleteSaleType
{
    public class DeleteSaleTypeCommandValidator : AbstractValidator<DeleteSaleTypeCommand>
    {
        public DeleteSaleTypeCommandValidator()
        {
            RuleFor(st => st.Id)
           .NotNull()
           .WithMessage("Id is required")
           .GreaterThan(0)
           .WithMessage("Id must be greater than 0");
        }
    }
}
