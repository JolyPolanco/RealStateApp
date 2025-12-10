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
    public class DefaultCLientUser
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager)
        {
            var defaultUser = new AppUser
            {
                UserName = "client",
                Email = "client@app.com",
                FirstName = "Default",
                LastName = "Client",
                EmailConfirmed = true, // no requiere confirmación en seed
                IsActive = true,
                Photo = "https://i.pinimg.com/originals/33/2b/c7/332bc749fcb06a540e08bca788301e71.jpg"

            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Client123!");
                await userManager.AddToRoleAsync(defaultUser, AppRoles.CLIENT.ToString());
            }

        }
    }
}
