using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealStateApp.Core.Application.Features.SaleType.Queries.GetAll;
using RealStateApp.Core.Application.Features.SaleType.Queries.GetAllWithInclude;
using RealStateApp.Core.Application.Mappings.EntitiesAndDtos;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Infraestructure.Identity.Contexts;
using RealStateApp.Infraestructure.Identity.Entities;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealStateApp.Unit.Tests.Features.SaleType.Queries
{
    public class GetAllWithIncludeSaleTypesQueryHandlerTests
    {


        private readonly DbContextOptions<RealStateContext> _dbOptions;
        private readonly DbContextOptions<IdentityContext> _IdentitydbOptions;
        private readonly IMapper _mapper;
        public GetAllWithIncludeSaleTypesQueryHandlerTests()
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
        public async Task Handle_ShouldReturnListWithInclude_WhenSaleTypesExist()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);
            using var identityContext = new IdentityContext(_IdentitydbOptions);

            // --- Seed de SaleTypes ---
            context.SaleTypes.AddRange(
                new RealStateApp.Core.Domain.Entities.SaleType { Id = 1, Name = "Sale type1", Description = "Description1" },
                new RealStateApp.Core.Domain.Entities.SaleType { Id = 2, Name = "Sale type2", Description = "Description2" }
            );
            await context.SaveChangesAsync();

            // --- Crear rol AGENT (para evitar roleId null) ---
            identityContext.Roles.Add(new IdentityRole
            {
                Name = AppRoles.AGENT.ToString(),
                NormalizedName = AppRoles.AGENT.ToString().ToUpper()
            });
            await identityContext.SaveChangesAsync();

            // --- Crear PropertyType ---
            var repoPropertyType = new PropertyTypeRepository(context);
            var propertyType = await repoPropertyType.AddAsync(new RealStateApp.Core.Domain.Entities.PropertyType
            {
                Name = "PropertyType1",
                Description = "PropertyType1 Description"
            });

            propertyType.Id.Should().BeGreaterThan(0);

            // --- Crear agente ---
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

            // Obtener rol AGENT
            var roleId = identityContext.Roles
                .Where(r => r.Name == AppRoles.AGENT.ToString())
                .Select(r => r.Id)
                .FirstOrDefault();

            roleId.Should().NotBeNull();

            // Asignarlo
            identityContext.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = agent.Id,
                RoleId = roleId!
            });
            await identityContext.SaveChangesAsync();

            // --- Crear Property ---
            context.Properties.Add(new RealStateApp.Core.Domain.Entities.Property
            {
                AgentId = agent.Id,
                Bathrooms = 2,
                Price = 233,
                Code = "12344",
                Bedrooms = 2,
                Description = "Decription",
                SizeInMeters = 120,
                Status = PropertyStatus.Available,
                PropertyTypeId = propertyType.Id,
                SaleTypeId = 1
            });

            await context.SaveChangesAsync();

            // Act
            var repository = new SaleTypeRepository(context);
            var handler = new GetAllSaleTypeWithIncludeQueryHandler(repository, _mapper);

            var result = await handler.Handle(new GetAllSaleTypeWithIncludeQuery(), CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);

            result.Select(r => r.Name)
                  .Should()
                  .Contain(new[] { "Sale type1", "Sale type2" });

            var saleType1 = result.First(r => r.Id == 1);
            var saleType2 = result.First(r => r.Id == 2);

            saleType1.PropertiesCount.Should().Be(1);
            saleType2.PropertiesCount.Should().Be(0);
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
