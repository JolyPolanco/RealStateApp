using Microsoft.AspNetCore.Identity;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Infraestructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Infraestructure.Identity.Seeds
{
    public class DefaultDeveloperUser
    {
  
            public static async Task SeedAsync(UserManager<AppUser> userManager)
            {
                var defaultUser = new AppUser
                {
                    UserName = "developer",
                    Email = "dev@app.com",
                    FirstName = "Default",
                    LastName = "Developer",
                    EmailConfirmed = true,
                    IsActive = true
                };

                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Developer123!");
                    await userManager.AddToRoleAsync(defaultUser, AppRoles.DEVELOPER.ToString());
                }
            }
        }
    }



