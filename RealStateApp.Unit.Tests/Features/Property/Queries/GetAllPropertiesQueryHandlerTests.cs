using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using RealStateApp.Core.Application.Dtos.User;
using RealStateApp.Core.Application.Features.Property.Queries.GetAll;
using RealStateApp.Core.Application.Interfaces;
using RealStateApp.Core.Application.Mappings.EntitiesAndDtos;
using RealStateApp.Core.Domain.Common.Enums;
using RealStateApp.Infraestructure.Identity.Contexts;
using RealStateApp.Infraestructure.Identity.Entities;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;

namespace RealStateApp.Unit.Tests.Features.Property.Queries
{
    public class GetAllPropertiesQueryHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbOptions;
        private readonly DbContextOptions<IdentityContext> _identityDbOptions;
        private readonly IMapper _mapper;
        private readonly Mock<IUserService> _userServiceMock;

        public GetAllPropertiesQueryHandlerTests()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PropertyToDtoMappingProfile>();
                cfg.AddProfile<PropertyTypeToDtoMappingProfile>();
                cfg.AddProfile<SaleTypeToDtoMappingProfile>();
                cfg.AddProfile<ImprovementToDtoMappingProfile>();
            }, loggerFactory);
            _mapper = mapperConfig.CreateMapper();

            _dbOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _identityDbOptions = new DbContextOptionsBuilder<IdentityContext>()
                .UseInMemoryDatabase(databaseName: $"IdentityTestDb_{Guid.NewGuid()}")
                .Options;

            // Mock IUserService
            _userServiceMock = new Mock<IUserService>();
        }

        [Fact]
        public async Task Handle_ShouldReturnAllProperties_WhenPropertiesExist()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);
            using var identityContext = new IdentityContext(_identityDbOptions);

            // Crear agente
            var agent = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "John",
                LastName = "Doe",
                Dni = "12345678901",
                UserName = "johndoe",
                Email = "john@example.com",
                PhoneNumber = "8091234567"
            };

            identityContext.Users.Add(agent);
            await identityContext.SaveChangesAsync();

            // Crear PropertyType y SaleType
            var propertyType = new Core.Domain.Entities.PropertyType
            {
                Id = 1,
                Name = "Casa",
                Description = "Casa familiar"
            };

            var saleType = new Core.Domain.Entities.SaleType
            {
                Id = 1,
                Name = "Venta",
                Description = "Venta directa"
            };

            context.PropertyTypes.Add(propertyType);
            context.SaleTypes.Add(saleType);
            await context.SaveChangesAsync();

            // Crear Improvement
            var improvement = new Core.Domain.Entities.Improvement
            {
                Id = 1,
                Name = "Piscina",
                Description = "Piscina privada"
            };

            context.Improvements.Add(improvement);
            await context.SaveChangesAsync();

            // Crear Properties
            var property1 = new Core.Domain.Entities.Property
            {
                Id = 1,
                Code = "ABC123",
                Price = 150000,
                SizeInMeters = 120.5,
                Bedrooms = 3,
                Bathrooms = 2,
                Description = "Casa hermosa",
                Status = PropertyStatus.Available,
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = agent.Id
            };

            var property2 = new Core.Domain.Entities.Property
            {
                Id = 2,
                Code = "XYZ789",
                Price = 200000,
                SizeInMeters = 150.0,
                Bedrooms = 4,
                Bathrooms = 3,
                Description = "Casa espaciosa",
                Status = PropertyStatus.Sold,
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = agent.Id
            };

            context.Properties.AddRange(property1, property2);
            await context.SaveChangesAsync();

            // Agregar PropertyImprovement
            var propertyImprovement = new Core.Domain.Entities.PropertyImprovement
            {
                PropertyId = 1,
                ImprovementId = 1
            };

            context.PropertyImprovements.Add(propertyImprovement);
            await context.SaveChangesAsync();

            // Configurar UserService mock
            _userServiceMock.Setup(us => us.GetById(agent.Id))
                .ReturnsAsync(new UserDto
                {
                    Id = agent.Id,
                    FirstName = agent.FirstName,
                    LastName = agent.LastName,
                    Email = agent.Email,
                    PhoneNumber = agent.PhoneNumber,
                    UserName = agent.UserName,
                    Dni = agent.Dni,
                    Role = "Agent"
                });

            var repository = new PropertyRepository(context);
            var handler = new GetAllPropertiesQueryHandler(repository, _mapper, _userServiceMock.Object);

            // Act
            var result = await handler.Handle(new GetAllPropertiesQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            var firstProperty = result.First(p => p.Code == "ABC123");
            firstProperty.PropertyType.Should().Be("Casa");
            firstProperty.SaleType.Should().Be("Venta");
            firstProperty.AgentName.Should().Be("John Doe");
            firstProperty.Improvements.Should().Contain("Piscina");
            firstProperty.Status.Should().Be(PropertyStatus.Available);

            var secondProperty = result.First(p => p.Code == "XYZ789");
            secondProperty.Bedrooms.Should().Be(4);
            secondProperty.Status.Should().Be(PropertyStatus.Sold);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoPropertiesExist()
        {
            // Arrange
            using var context = new RealStateContext(_dbOptions);

            var repository = new PropertyRepository(context);
            var handler = new GetAllPropertiesQueryHandler(repository, _mapper, _userServiceMock.Object);

            // Act
            var result = await handler.Handle(new GetAllPropertiesQuery(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}
