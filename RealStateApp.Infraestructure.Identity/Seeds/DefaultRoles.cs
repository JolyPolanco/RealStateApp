using Microsoft.AspNetCore.Identity;
using RealStateApp.Core.Domain.Common.Enums;

namespace RealStateApp.Infraestructure.Identity.Seeds
{
    public class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {

           
            await roleManager.CreateAsync(new IdentityRole(AppRoles.ADMIN.ToString()));
            await roleManager.CreateAsync(new IdentityRole(AppRoles.CLIENT.ToString()));
            await roleManager.CreateAsync(new IdentityRole(AppRoles.AGENT.ToString()));
            await roleManager.CreateAsync(new IdentityRole(AppRoles.DEVELOPER.ToString()));

        }
    }
    
}
