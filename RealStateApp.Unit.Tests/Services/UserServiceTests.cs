using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Unit.Tests.Services
{
    using FluentAssertions;
    using global::RealStateApp.Core.Domain.Common.Enums;
    using global::RealStateApp.Infraestructure.Identity.Contexts;
    using global::RealStateApp.Infraestructure.Identity.Entities;
    using global::RealStateApp.Infraestructure.Identity.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

 
        public class UserServiceTests
        {
            private static (IdentityContext ctx, UserManager<AppUser> um, RoleManager<IdentityRole> rm) CreateIdentityWithManagers(string dbName)
            {
                var options = new DbContextOptionsBuilder<IdentityContext>()
                    .UseInMemoryDatabase(databaseName: dbName)
                    .Options;

                var context = new IdentityContext(options);

                // stores
                var userStore = new UserStore<AppUser, IdentityRole, IdentityContext>(context);
                var roleStore = new RoleStore<IdentityRole, IdentityContext>(context);

                // user manager dependencies
                var userValidators = new List<IUserValidator<AppUser>> { new UserValidator<AppUser>() };
                var pwdValidators = new List<IPasswordValidator<AppUser>> { new PasswordValidator<AppUser>() };

                var lookupNormalizer = new UpperInvariantLookupNormalizer();
                var errors = new IdentityErrorDescriber();
                var pwdHasher = new PasswordHasher<AppUser>();
                var services = new Mock<IServiceProvider>().Object;
                var loggerUser = new Logger<UserManager<AppUser>>(new LoggerFactory());
                var loggerRole = new Logger<RoleManager<IdentityRole>>(new LoggerFactory());

                var userManager = new UserManager<AppUser>(
                    userStore,
                    Options.Create(new IdentityOptions()),
                    pwdHasher,
                    userValidators,
                    pwdValidators,
                    lookupNormalizer,
                    errors,
                    services,
                    loggerUser);

                var roleManager = new RoleManager<IdentityRole>(
                    roleStore,
                    new[] { new RoleValidator<IdentityRole>() },
                    lookupNormalizer,
                    errors,
                    loggerRole);

                return (context, userManager, roleManager);
            }

            private static IdentityUserService CreateServiceWithSeededData(
                string dbName,
                out IdentityContext identityContext,
                out UserManager<AppUser> userManager,
                out RoleManager<IdentityRole> roleManager)
            {
                (identityContext, userManager, roleManager) = CreateIdentityWithManagers(dbName);

                // Ensure DB created
                identityContext.Database.EnsureDeleted();
                identityContext.Database.EnsureCreated();

                var httpContextAccessor = new Mock<IHttpContextAccessor>().Object;
                var service = new IdentityUserService(userManager, identityContext, httpContextAccessor);
                return service;
            }

            [Fact]
            public async Task DeleteAsync_ShouldReturnError_WhenUserNotFound()
            {
                var service = CreateServiceWithSeededData(nameof(DeleteAsync_ShouldReturnError_WhenUserNotFound),
                    out var ctx, out var userManager, out var roleManager);

                // No users seeded

                var result = await service.DeleteAsync("nonexistent-id");
                result.HasError.Should().BeTrue();
                result.Errors.Should().Contain("No existe un usuario con este ID.");
            }

            [Fact]
            public async Task DeleteAsync_ShouldDeleteUser_WhenExists()
            {
                var service = CreateServiceWithSeededData(nameof(DeleteAsync_ShouldDeleteUser_WhenExists),
                    out var ctx, out var userManager, out var roleManager);

                // seed role and user via managers to ensure all user-role relations work
                var role = new IdentityRole(AppRoles.AGENT.ToString());
                await roleManager.CreateAsync(role);

                var user = new AppUser
                {
                    UserName = "agent1",
                    Email = "agent1@test.com",
                    FirstName = "A",
                    LastName = "B",
                    Dni = "111",
                    IsActive = true
                };
                await userManager.CreateAsync(user, "Password!1");
                await userManager.AddToRoleAsync(user, AppRoles.AGENT.ToString());

                var res = await service.DeleteAsync(user.Id);

                res.HasError.Should().BeFalse();
                var found = await userManager.FindByIdAsync(user.Id);
                found.Should().BeNull(); // deleted
            }

            [Fact]
            public async Task ToogleState_ShouldInvertIsActive()
            {
                var service = CreateServiceWithSeededData(nameof(ToogleState_ShouldInvertIsActive),
                    out var ctx, out var userManager, out var roleManager);

                var user = new AppUser { UserName = "u1", Email = "u1@test", IsActive = true, FirstName="User1", LastName="User" };
                await userManager.CreateAsync(user, "Password!1");

                await service.ToogleState(user.Id);

                var updated = await userManager.FindByIdAsync(user.Id);
                updated.IsActive.Should().BeFalse();

                // toggle again
                await service.ToogleState(user.Id);
                updated = await userManager.FindByIdAsync(user.Id);
                updated.IsActive.Should().BeTrue();
            }

            [Fact]
            public async Task SetStatus_ShouldReturnTrue_WhenUserExists()
            {
                var service = CreateServiceWithSeededData(nameof(SetStatus_ShouldReturnTrue_WhenUserExists),
                    out var ctx, out var userManager, out var roleManager);

                var user = new AppUser { UserName = "u2", Email = "u2@test", IsActive = false , FirstName = "User1", LastName = "User" };
                await userManager.CreateAsync(user, "Password!1");

                var ok = await service.SetStatus(user.Id, true);
                ok.Should().BeTrue();

                var updated = await userManager.FindByIdAsync(user.Id);
                updated.IsActive.Should().BeTrue();
            }

            [Fact]
            public async Task GetByDni_ShouldReturnUserDto_WhenExists()
            {
                var service = CreateServiceWithSeededData(nameof(GetByDni_ShouldReturnUserDto_WhenExists),
                    out var ctx, out var userManager, out var roleManager);

                // seed role and user
                var agentRole = new IdentityRole(AppRoles.AGENT.ToString());
                await roleManager.CreateAsync(agentRole);

                var user = new AppUser
                {
                    UserName = "dniUser",
                    Email = "dni@test",
                    FirstName = "Fn",
                    LastName = "Ln",
                    Dni = "00-123 456"
                };
                await userManager.CreateAsync(user, "Password!1");
                await userManager.AddToRoleAsync(user, AppRoles.AGENT.ToString());

                var dto = await service.GetByDni("00123456");
                dto.Should().NotBeNull();
                dto!.Id.Should().Be(user.Id);
                dto.Role.Should().Be(AppRoles.AGENT.ToString());
            }

            [Fact]
            public async Task GetById_ShouldReturnUserDto_WhenExists()
            {
                var service = CreateServiceWithSeededData(nameof(GetById_ShouldReturnUserDto_WhenExists),
                    out var ctx, out var userManager, out var roleManager);

                var role = new IdentityRole(AppRoles.CLIENT.ToString());
                await roleManager.CreateAsync(role);

                var user = new AppUser
                {
                    UserName = "uget",
                    Email = "uget@test",
                    FirstName = "F",
                    LastName = "L",
                    Dni = "888"
                };
                await userManager.CreateAsync(user, "Password!1");
                await userManager.AddToRoleAsync(user, AppRoles.CLIENT.ToString());

                var dto = await service.GetById(user.Id);
                dto.Should().NotBeNull();
                dto!.Id.Should().Be(user.Id);
                dto.Role.Should().Be(AppRoles.CLIENT.ToString());
            }

            [Fact]
            public async Task GetUsersAgentOnly_ShouldReturnAgentsWithCounts()
            {
                var service = CreateServiceWithSeededData(nameof(GetUsersAgentOnly_ShouldReturnAgentsWithCounts),
                    out var ctx, out var userManager, out var roleManager);

                // create role and users
                var role = new IdentityRole(AppRoles.AGENT.ToString());
                await roleManager.CreateAsync(role);

                var u1 = new AppUser { UserName = "a1", Email = "a1@test", FirstName = "A1", LastName = "L1", Dni = "1" };
                var u2 = new AppUser { UserName = "a2", Email = "a2@test", FirstName = "A2", LastName = "L2", Dni = "2" };
                await userManager.CreateAsync(u1, "Password!1");
                await userManager.CreateAsync(u2, "Password!1");
                await userManager.AddToRoleAsync(u1, AppRoles.AGENT.ToString());
                await userManager.AddToRoleAsync(u2, AppRoles.AGENT.ToString());

                var dict = new Dictionary<string, int>
                {
                    [u1.Id] = 3,
                    [u2.Id] = 0
                };

                var list = await service.GetUsersAgentOnly(dict);
                list.Should().HaveCount(2);
                list.First(l => l.Id == u1.Id).PropertiesCount.Should().Be(3);
                list.First(l => l.Id == u2.Id).PropertiesCount.Should().Be(0);
            }

            [Fact]
            public async Task GetUsersByRole_ShouldReturnUsersWithDisplayRole()
            {
                var service = CreateServiceWithSeededData(nameof(GetUsersByRole_ShouldReturnUsersWithDisplayRole),
                    out var ctx, out var userManager, out var roleManager);

                var role = new IdentityRole("DEVELOPER");
                await roleManager.CreateAsync(role);

                var u = new AppUser { UserName = "dev", Email = "dev@test", FirstName = "D", LastName = "V", Dni = "9" };
                await userManager.CreateAsync(u, "Password!1");
                await userManager.AddToRoleAsync(u, "DEVELOPER");

                var res = await service.GetUsersByRole("DEVELOPER", "Developer");
                res.Should().HaveCount(1);
                res[0].Role.Should().Be("Developer");
            }

            [Fact]
            public async Task GetUsersAdminOnly_GetUsersDevelopersOnly_GetClientsDevelopersOnly_CountsWork()
            {
                var service = CreateServiceWithSeededData(nameof(GetUsersAdminOnly_GetUsersDevelopersOnly_GetClientsDevelopersOnly_CountsWork),
                    out var ctx, out var userManager, out var roleManager);

                // roles
                await roleManager.CreateAsync(new IdentityRole("ADMIN"));
                await roleManager.CreateAsync(new IdentityRole("DEVELOPER"));
                await roleManager.CreateAsync(new IdentityRole("CLIENT"));

                // users
                var a = new AppUser { UserName = "ad", Email = "ad@test", Dni = "10", IsActive = true , FirstName = "User1", LastName = "User" };
                var d = new AppUser { UserName = "dev", Email = "dev@test", Dni = "11", IsActive = false, FirstName = "User1", LastName = "User" };
                var c = new AppUser {UserName = "cli", Email = "cli@test", Dni = "12", IsActive = true, FirstName = "User1", LastName = "User" };

                await userManager.CreateAsync(a, "P@ssw0rd!");
                await userManager.CreateAsync(d, "P@ssw0rd!");
                await userManager.CreateAsync(c, "P@ssw0rd!");

                await userManager.AddToRoleAsync(a, "ADMIN");
                await userManager.AddToRoleAsync(d, "DEVELOPER");
                await userManager.AddToRoleAsync(c, "CLIENT");

                var admins = await service.GetUsersAdminOnly();
                var developers = await service.GetUsersDevelopersOnly();
                var clients = await service.GetClientsDevelopersOnly();

                admins.Should().HaveCount(1);
                developers.Should().HaveCount(1);
                clients.Should().HaveCount(1);
            }

            [Fact]
            public async Task ActiveInactiveCounts_ShouldReturnCorrectNumbers()
            {
                var service = CreateServiceWithSeededData(nameof(ActiveInactiveCounts_ShouldReturnCorrectNumbers),
                    out var ctx, out var userManager, out var roleManager);

                await roleManager.CreateAsync(new IdentityRole("AGENT"));
                await roleManager.CreateAsync(new IdentityRole("CLIENT"));

                var a1 = new AppUser { UserName = "a1", Email = "a1@test", Dni = "a1", IsActive = true , FirstName = "User1", LastName = "User" };
                var a2 = new AppUser { UserName = "a2", Email = "a2@test", Dni = "a2", IsActive = false , FirstName = "User1", LastName = "User" };
                var c1 = new AppUser { UserName = "c1", Email = "c1@test", Dni = "c1", IsActive = true , FirstName = "User1", LastName = "User" };

                await userManager.CreateAsync(a1, "Password!1");
                await userManager.CreateAsync(a2, "Password!1");
                await userManager.CreateAsync(c1, "Password!1");

                await userManager.AddToRoleAsync(a1, "AGENT");
                await userManager.AddToRoleAsync(a2, "AGENT");
                await userManager.AddToRoleAsync(c1, "CLIENT");

                var activeAgents = await service.GetActiveAgentsCount();
                var inactiveAgents = await service.GetInactiveAgentsCount();
                var activeClients = await service.GetActiveClientsCount();
                var inactiveClients = await service.GetInactiveClientsCount();

                activeAgents.Should().Be(1);
                inactiveAgents.Should().Be(1);
                activeClients.Should().Be(1);
                inactiveClients.Should().Be(0);
            }
        }
    }


