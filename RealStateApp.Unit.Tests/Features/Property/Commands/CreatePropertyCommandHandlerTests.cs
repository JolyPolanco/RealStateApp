using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.Property.Commands.CreateProperty;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Core.Domain.Interfaces;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;

namespace RealStateApp.Unit.Tests.Features.Property.Commands
{
    public class CreatePropertyCommandHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbContextOptions;

        public CreatePropertyCommandHandlerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"RealStateAppTest_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_ShouldReturnPropertyId_WhenCreationIsSuccessful()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            
            var propertyType = new Core.Domain.Entities.PropertyType { Id = 1, Name = "Casa", Description = "Casa" };
            var saleType = new Core.Domain.Entities.SaleType { Id = 1, Name = "Venta", Description = "Venta" };
            
            await context.PropertyTypes.AddAsync(propertyType);
            await context.SaleTypes.AddAsync(saleType);
            await context.SaveChangesAsync();

            var propertyRepository = new PropertyRepository(context);
            var propertyTypeRepository = new PropertyTypeRepository(context);
            var saleTypeRepository = new SaleTypeRepository(context);

            var handler = new CreatePropertyCommandHandler(
                propertyRepository,
                propertyTypeRepository,
                saleTypeRepository);

            var command = new CreatePropertyCommand
            {
                Price = 100000,
                SizeInMeters = 150.5,
                Bedrooms = 3,
                Bathrooms = 2,
                Description = "Beautiful property",
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = "agent123",
                ImprovementIds = new List<int>()
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0);
            var createdProperty = await context.Properties.FindAsync(result);
            createdProperty.Should().NotBeNull();
            createdProperty!.Code.Should().HaveLength(6);
            createdProperty.Price.Should().Be(command.Price);
            createdProperty.SizeInMeters.Should().Be(command.SizeInMeters);
            createdProperty.Bedrooms.Should().Be(command.Bedrooms);
            createdProperty.Bathrooms.Should().Be(command.Bathrooms);
            createdProperty.Description.Should().Be(command.Description);
            createdProperty.PropertyTypeId.Should().Be(command.PropertyTypeId);
            createdProperty.SaleTypeId.Should().Be(command.SaleTypeId);
            createdProperty.AgentId.Should().Be(command.AgentId);
        }

        [Fact]
        public async Task Handle_ShouldThrowApiException_WhenPropertyTypeDoesNotExist()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            
            var saleType = new Core.Domain.Entities.SaleType { Id = 1, Name = "Venta", Description = "Venta" };
            await context.SaleTypes.AddAsync(saleType);
            await context.SaveChangesAsync();

            var propertyRepository = new PropertyRepository(context);
            var propertyTypeRepository = new PropertyTypeRepository(context);
            var saleTypeRepository = new SaleTypeRepository(context);

            var handler = new CreatePropertyCommandHandler(
                propertyRepository,
                propertyTypeRepository,
                saleTypeRepository);

            var command = new CreatePropertyCommand
            {
                Price = 100000,
                SizeInMeters = 150.5,
                Bedrooms = 3,
                Bathrooms = 2,
                Description = "Beautiful property",
                PropertyTypeId = 999, // Non-existent
                SaleTypeId = 1,
                AgentId = "agent123",
                ImprovementIds = new List<int>()
            };

            // Act & Assert
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("El tipo de propiedad no existe");
        }

        [Fact]
        public async Task Handle_ShouldThrowApiException_WhenSaleTypeDoesNotExist()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            
            var propertyType = new Core.Domain.Entities.PropertyType { Id = 1, Name = "Casa", Description = "Casa" };
            await context.PropertyTypes.AddAsync(propertyType);
            await context.SaveChangesAsync();

            var propertyRepository = new PropertyRepository(context);
            var propertyTypeRepository = new PropertyTypeRepository(context);
            var saleTypeRepository = new SaleTypeRepository(context);

            var handler = new CreatePropertyCommandHandler(
                propertyRepository,
                propertyTypeRepository,
                saleTypeRepository);

            var command = new CreatePropertyCommand
            {
                Price = 100000,
                SizeInMeters = 150.5,
                Bedrooms = 3,
                Bathrooms = 2,
                Description = "Beautiful property",
                PropertyTypeId = 1,
                SaleTypeId = 999, // Non-existent
                AgentId = "agent123",
                ImprovementIds = new List<int>()
            };

            // Act & Assert
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("El tipo de venta no existe");
        }
    }
}
