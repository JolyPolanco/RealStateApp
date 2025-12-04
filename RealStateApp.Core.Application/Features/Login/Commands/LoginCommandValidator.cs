using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Core.Application.Features.Login.Commands
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
            {
                RuleFor(d => d.Username)
                    .NotEmpty().WithMessage("Username is required");

                RuleFor(d => d.Password)
                    .NotEmpty().WithMessage("Password is required");
            }
        

    }
}
