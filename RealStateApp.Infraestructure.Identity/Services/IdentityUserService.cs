using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg.OpenPgp;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Helpers;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Infraestructure.Identity.Contexts;
using RealStateApp.Infraestructure.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Infraestructure.Identity.Services
{
    public class IdentityUserService : IUserService

    {
        private readonly IdentityContext _context;
        private UserManager<AppUser> _userManager;
        private readonly IdentityContext _identityContext;
        public IdentityUserService(UserManager<AppUser> userManager, IdentityContext identityDbContext, IHttpContextAccessor context)
        {
            _userManager = userManager;
            _identityContext = identityDbContext;
            _identityContext = identityDbContext;

        }


        public async Task<UserDto?> GetByDni(string dni)
        {
            var cleanDocumentId = dni?.Trim().Replace("-", "").Replace(" ", "") ?? "";

            var user = await _userManager.Users
                .Where(r => r.Dni.Replace("-", "").Replace(" ", "") == cleanDocumentId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return null;
            }

            var rolesList = await _userManager.GetRolesAsync(user);
            var role = EnumMapper<AppRoles>.FromString(rolesList.First());

            var userDto = new UserDto()
            {
                Id = user.Id,
                Email = user.Email ?? "",
                LastName = user.LastName,
                FirstName = user.FirstName,
                UserName = user.UserName ?? "",
                Dni = user.Dni!,
                IsVerified = user.EmailConfirmed,
                IsActive = user.IsActive,
                Role = EnumMapper<AppRoles>.ToString(role)
            };

            return userDto;
        }


        public async Task<List<UserDto>> GetUsersAgentsOnly()
        {
            var agents = await _userManager.GetUsersInRoleAsync(AppRoles.AGENT.ToString());

            return agents
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Dni = user.Dni!,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsActive = user.IsActive,
                    UserName = user.UserName!,
                    Role = "Agente",
                    IsVerified = user.EmailConfirmed
                })
                .ToList();
        }

        public async Task<List<UserDto>> GetUsersByRole(string role, string displayRole)
        {

            var users = await _userManager.GetUsersInRoleAsync(role);

            return users
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Dni = user.Dni!,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    IsActive = user.IsActive,
                    UserName = user.UserName!,
                    Role = displayRole,
                    IsVerified = user.EmailConfirmed
                })
                .ToList();
        }

        public async Task<List<UserDto>> GetUsersAdminOnly()
        {
            return await GetUsersByRole("ADMIN", EnumMapper<AppRoles>.ToString(AppRoles.ADMIN));

        }
        public async Task<List<UserDto>> GetUsersAgentOnly()
        {
            return await GetUsersByRole("AGENT", EnumMapper<AppRoles>.ToString(AppRoles.AGENT));

        }
        public async Task<List<UserDto>> GetUsersDevelopersOnly()
        {
            return await GetUsersByRole("DEVELOPER", EnumMapper<AppRoles>.ToString(AppRoles.DEVELOPER));

        }

        public async Task<List<UserDto>> GetClientsDevelopersOnly()
        {
            return await GetUsersByRole("CLIENT", EnumMapper<AppRoles>.ToString(AppRoles.CLIENT));

        }

    }

}
