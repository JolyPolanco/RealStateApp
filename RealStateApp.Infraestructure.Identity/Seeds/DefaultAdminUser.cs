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
    public class DefaultAdminUser
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager)
        {
            var defaultUser = new AppUser
            {
                UserName = "admin",
                Email = "admin@app.com",
                FirstName = "System",
                LastName = "Administrator",
                EmailConfirmed = true,
                IsActive = true
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);

            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Admin123!");
                await userManager.AddToRoleAsync(defaultUser, AppRoles.ADMIN.ToString());
            }
        }
    }
}
