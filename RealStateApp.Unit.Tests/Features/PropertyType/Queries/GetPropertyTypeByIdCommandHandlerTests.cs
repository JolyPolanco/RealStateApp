using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.PropertyType.Queries.GetAll;
using RealStateApp.Core.Application.Features.PropertyType.Queries.GetById;
using RealStateApp.Core.Application.Features.SaleType.Queries.GetById;
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
    public class GetPropertyTypeByIdCommandHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbOptions;
        private DbContextOptions<IdentityContext> _IdentitydbOptions;
        private IMapper _mapper;

        public GetPropertyTypeByIdCommandHandlerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"InMemoryDb_{Guid.NewGuid()}")
                .Options;

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<SaleTypeToDtoMappingProfile>();
            }, loggerFactory);
            _IdentitydbOptions = new DbContextOptionsBuilder<IdentityContext>()
             .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
             .Options;
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Handle_ShouldReturnPropertyTypeResponseDto_WhenIdExists()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);
            using var identityContext = new IdentityContext(_IdentitydbOptions);

            // --- Seed de SaleTypes ---
            context.SaleTypes.AddRange(
                new Core.Domain.Entities.SaleType { Id = 1, Name = "Sale type1", Description = "Description1" },
                new Core.Domain.Entities.SaleType { Id = 2, Name = "Sale type2", Description = "Description2" }
            );
            await context.SaveChangesAsync();

            // --- Seed de PropertyTypes ---
            context.PropertyTypes.AddRange(
                new Core.Domain.Entities.PropertyType { Id = 1, Name = "Property Type1", Description = "Description1" },
                new Core.Domain.Entities.PropertyType { Id = 2, Name = "Property Type2", Description = "Description2" }
            );
            await context.SaveChangesAsync();

            // Crear PropertyType vía repositorio
            var repoPropertyType = new PropertyTypeRepository(context);
            var propertyType = await repoPropertyType.AddAsync(new RealStateApp.Core.Domain.Entities.PropertyType
            {
                Name = "PropertyType1",
                Description = "PropertyType1 Description"
            });
            propertyType!.Id.Should().BeGreaterThan(0);

            // --- Crear rol AGENT (evita error roleId null) ---
            identityContext.Roles.Add(new IdentityRole
            {
                Name = AppRoles.AGENT.ToString(),
                NormalizedName = AppRoles.AGENT.ToString().ToUpper()
            });
            await identityContext.SaveChangesAsync();

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

            var roleId = identityContext.Roles
                .Where(r => r.Name == AppRoles.AGENT.ToString())
                .Select(r => r.Id)
                .FirstOrDefault();
            roleId.Should().NotBeNull();

            identityContext.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = agent.Id,
                RoleId = roleId!
            });
            await identityContext.SaveChangesAsync();

            // --- Crear Property asociada al SaleType 1 ---
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
                PropertyTypeId = propertyType.Id,
                SaleTypeId = 1
            });
            await context.SaveChangesAsync();

            // Act
            var repository = new SaleTypeRepository(context);
            var handler = new GetSaleTypeByIdQueryHandler(repository, _mapper);

            var result = await handler.Handle(new GetSaleTypeByIdQuery { Id = 1 }, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Sale type1");
            result.Description.Should().Be("Description1");
            result.PropertiesCount.Should().Be(1);
        }



        [Fact]
        public async Task Handle_ShouldThrowApiException_WhenIdDoesNotExist()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);
            var repository = new PropertyTypeRepository(context);
            var handler = new GetPropertyTypeByIdQueryHandler(repository, _mapper);

            var query = new GetPropertyTypeByIdQuery { Id = 999 };

            // Act
            Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);
            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Invalid Id");
        }


    
}
}
