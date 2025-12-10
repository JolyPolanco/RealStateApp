using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealStateApp.Core.Application.Features.PropertyType.Queries.GetAllWithInclude;
using RealStateApp.Core.Application.Features.SaleType.Queries.GetAllWithInclude;
using RealStateApp.Core.Application.Mappings.EntitiesAndDtos;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Infraestructure.Identity.Contexts;
using RealStateApp.Infraestructure.Identity.Entities;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Unit.Tests.Features.PropertyType.Queries
{
    public class GetAllPropertyTypesWithIncludeCommandHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbOptions;
        private readonly DbContextOptions<IdentityContext> _IdentitydbOptions;
        private readonly IMapper _mapper;
        public GetAllPropertyTypesWithIncludeCommandHandlerTests()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SaleTypeToDtoMappingProfile>();
                cfg.AddProfile<PropertyToDtoMappingProfile>();
                cfg.AddProfile<PropertyTypeToDtoMappingProfile>();
            }, loggerFactory);
            _mapper = mapperConfig.CreateMapper();

            _dbOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _IdentitydbOptions = new DbContextOptionsBuilder<IdentityContext>()
              .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
              .Options;

        }

        [Fact]
        public async Task GetAllPropertyTypeWithInclude_ShouldReturnListWithCorrectPropertyCounts()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);
            using var identityContext = new IdentityContext(_IdentitydbOptions);

            // Seed Roles (AGENT requerido)
            var agentRole = new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = AppRoles.AGENT.ToString(),
                NormalizedName = AppRoles.AGENT.ToString().ToUpper()
            };

            identityContext.Roles.Add(agentRole);
            await identityContext.SaveChangesAsync();

            // Seed PropertyTypes
            context.PropertyTypes.AddRange(
                new Core.Domain.Entities.PropertyType { Id = 1, Name = "Property Type1", Description = "Description1" },
                new Core.Domain.Entities.PropertyType { Id = 2, Name = "Property Type2", Description = "Description2" }
            );

            await context.SaveChangesAsync();

            // Seed agent
            var agent = new AppUser
            {
                FirstName = "Agent",
                LastName = "Agent",
                Dni = "12345678952",
                UserName = "Agente1",
                Email = "test@gmail.com"
            };

            identityContext.Users.Add(agent);
            await identityContext.SaveChangesAsync();

            // Assign AGENT role
            identityContext.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = agent.Id,
                RoleId = agentRole.Id
            });

            await identityContext.SaveChangesAsync();

            // Seed Property linked to PropertyType 1
            context.Properties.Add(new RealStateApp.Core.Domain.Entities.Property
            {
                AgentId = agent.Id,
                Bathrooms = 2,
                Price = 233,
                Code = "12344",
                Bedrooms = 2,
                Description = "Description",
                SizeInMeters = 120,
                Status = PropertyStatus.Available,
                PropertyTypeId = 1,
                SaleTypeId = 1
            });

            await context.SaveChangesAsync();

            // Act
            var repository = new PropertyTypeRepository(context);
            var handler = new GetAllPropertyTypeWithIncludeQueryHandler(repository, _mapper);

            var result = await handler.Handle(new GetAllPropertyTypeWithIncludeQuery(), CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);

            var ptype1 = result.First(r => r.Id == 1);
            var ptype2 = result.First(r => r.Id == 2);

            ptype1.PropertiesCount.Should().Be(1);
            ptype2.PropertiesCount.Should().Be(0);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoSaleTypesExist()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);
            var repository = new SaleTypeRepository(context);
            var handler = new GetAllSaleTypeWithIncludeQueryHandler(repository, _mapper);

            // Act
            var result = await handler.Handle(new GetAllSaleTypeWithIncludeQuery(), CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
