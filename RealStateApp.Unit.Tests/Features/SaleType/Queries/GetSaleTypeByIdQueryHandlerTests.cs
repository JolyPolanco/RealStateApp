using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.SaleType.Queries.GetById;
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
    public class GetSaleTypeByIdQueryHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbOptions;
        private DbContextOptions<IdentityContext> _IdentitydbOptions;
        private IMapper _mapper;

        public GetSaleTypeByIdQueryHandlerTests()
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
        public async Task Handle_ShouldReturnSaleTypeResponseDto_WhenIdExists()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);
            using var identityContext = new IdentityContext(_IdentitydbOptions);

            // SaleTypes
            context.SaleTypes.AddRange(
                new RealStateApp.Core.Domain.Entities.SaleType { Id = 1, Name = "Sale type1", Description = "Description1" },
                new RealStateApp.Core.Domain.Entities.SaleType { Id = 2, Name = "Sale type2", Description = "Description2" }
            );

            await context.SaveChangesAsync();
            context.SaleTypes.Count().Should().Be(2);

            // Crear PropertyType
            var repoPropertyType = new PropertyTypeRepository(context);
            var propertyType = await repoPropertyType.AddAsync(new RealStateApp.Core.Domain.Entities.PropertyType
            {
                Name = "PropertyType1",
                Description = "PropertyType1 Description"
            });

            propertyType!.Id.Should().BeGreaterThan(0);
            context.PropertyTypes.Any(pt => pt.Id == propertyType.Id).Should().BeTrue();

            // Crear agente
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

            identityContext.Users.Any(u => u.Id == agent.Id).Should().BeTrue();

            var roleId = identityContext.Roles
                .Where(r => r.Name == AppRoles.AGENT.ToString())
                .Select(r => r.Id)
                .FirstOrDefault();

            roleId.Should().NotBeNull();

            identityContext.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = agent.Id,
                RoleId = roleId
            });

            await identityContext.SaveChangesAsync();

            identityContext.UserRoles
                .Any(ur => ur.UserId == agent.Id && ur.RoleId == roleId)
                .Should()
                .BeTrue();

            // Crear Property asociada al SaleType 1
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

            var createdProperty = context.Properties.FirstOrDefault();
            createdProperty.Should().NotBeNull();
            createdProperty!.SaleTypeId.Should().Be(1);

            // Act
            var repository = new SaleTypeRepository(context);
            var handler = new GetSaleTypeByIdQueryHandler(repository, _mapper);

            var query = new GetSaleTypeByIdQuery { Id = 1 };

            var result = await handler.Handle(query, CancellationToken.None);

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
            var repository = new SaleTypeRepository(context);
            var handler = new GetSaleTypeByIdQueryHandler(repository, _mapper);

            var query = new GetSaleTypeByIdQuery { Id = 999 };

            // Act
            Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);
            // Assert
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("Sale Type not found with Id");
        }


    }
}
