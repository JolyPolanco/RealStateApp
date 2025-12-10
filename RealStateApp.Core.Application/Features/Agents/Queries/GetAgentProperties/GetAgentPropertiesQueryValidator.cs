using FluentValidation;

namespace RealStateApp.Core.Application.Features.Agents.Queries.GetAgentProperties
{
    public class GetAgentPropertiesQueryValidator : AbstractValidator<GetAgentPropertiesQuery>
    {
        public GetAgentPropertiesQueryValidator()
        {
            RuleFor(x => x.AgentId)
                .NotEmpty().WithMessage("Agent ID is required")
                .NotNull().WithMessage("Agent ID is required");
        }
    }
}
