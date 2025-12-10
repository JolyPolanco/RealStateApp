using FluentValidation;

namespace RealStateApp.Core.Application.Features.Property.Queries.GetById
{
    public class GetPropertyByIdQueryValidator : AbstractValidator<GetPropertyByIdQuery>
    {
        public GetPropertyByIdQueryValidator()
        {
            RuleFor(r => r.Id)
                .NotNull()
                .WithMessage("Id is required")
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0");
        }
    }
}
