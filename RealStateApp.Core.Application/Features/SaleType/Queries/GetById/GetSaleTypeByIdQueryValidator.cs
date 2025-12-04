using FluentValidation;


namespace RealStateApp.Core.Application.Features.SaleType.Queries.GetById
{
    public class GetSaleTypeByIdQueryValidator : AbstractValidator<GetSaleTypeByIdQuery>
    {

        public GetSaleTypeByIdQueryValidator()
        {
            RuleFor(r=>r.Id)
                .NotNull()
                .WithMessage("Id is required")
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0");

        }
    }
}
