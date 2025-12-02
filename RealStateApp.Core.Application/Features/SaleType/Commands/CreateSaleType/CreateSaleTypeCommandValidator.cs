
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.SaleType.Commands.CreateSaleType
{
    public class CreateSaleTypeCommandValidator : AbstractValidator<CreateSaleTypeCommand>
    {
        public CreateSaleTypeCommandValidator()
        {
            RuleFor(st => st.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(150).
                WithMessage("Sale Type name must not exceed 150 characters");

            RuleFor(st => st.Description)
               .NotEmpty()
               .WithMessage("Description is required")
               .MaximumLength(250).WithMessage("Sale Type Description must not exceed 250 characters");

        }
    }
}
