using FluentValidation;
using RealStateApp.Core.Application.Features.Agents.Queries.GetById;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Improvement.Queries.GetById
{
    public class GetImprovementByIdQueryValidator : AbstractValidator<GetImprovementByIdQuery>
    
    {
        public GetImprovementByIdQueryValidator()
        {
            RuleFor(r => r.Id)
                    .NotNull()
                    .WithMessage("Id is required")
                    .GreaterThan(0)
                    .WithMessage("Id must be greater than 0");
        }

    }
}
