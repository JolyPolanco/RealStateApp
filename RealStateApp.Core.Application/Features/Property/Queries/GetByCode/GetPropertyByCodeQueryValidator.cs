using FluentValidation;

namespace RealStateApp.Core.Application.Features.Property.Queries.GetByCode
{
    public class GetPropertyByCodeQueryValidator : AbstractValidator<GetPropertyByCodeQuery>
    {
        public GetPropertyByCodeQueryValidator()
        {
            RuleFor(r => r.Code)
                .NotEmpty()
                .WithMessage("Code is required")
                .NotNull()
                .WithMessage("Code cannot be null")
                .Length(6, 6)
                .WithMessage("Code must be exactly 6 characters");
        }
    }
}
