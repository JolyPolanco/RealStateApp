using FluentValidation;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.PropertyType.Queries.GetById
{
    public class GetPropertyTypeByIdQueryValidator: AbstractValidator<GetPropertyTypeByIdQuery>
    {
        public GetPropertyTypeByIdQueryValidator()
        {
            RuleFor(r => r.Id)
             .NotNull()
             .WithMessage("Id is required")
             .GreaterThan(0)
             .WithMessage("Id must be greater than 0");

        }
    }
}
