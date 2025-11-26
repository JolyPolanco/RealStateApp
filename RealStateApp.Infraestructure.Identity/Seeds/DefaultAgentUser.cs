
using Microsoft.AspNetCore.Identity;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Infraestructure.Identity.Entities;

namespace RealStateApp.Infraestructure.Identity.Seeds
{
    public class DefaultAgentUser
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager)
        {
            var defaultUser = new AppUser
            {
                UserName = "agent",
                Email = "agent@app.com",
                FirstName = "Default",
                LastName = "Agent",
                EmailConfirmed = true,
                IsActive = true
            };

            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Agent123!");
                await userManager.AddToRoleAsync(defaultUser, AppRoles.AGENT.ToString());
            }

        }
    }
    }
