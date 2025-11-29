using Microsoft.AspNetCore.Identity;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Infraestructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

namespace RealStateApp.Infraestructure.Identity.Services
{
    public class AccountServiceForWebApi : BaseAccountService, IAccountServiceForWebApi
    {
        public AccountServiceForWebApi(UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager, IMapper mapper) : base(userManager, emailService, signInManager, mapper)
        {
        }
    }
}
