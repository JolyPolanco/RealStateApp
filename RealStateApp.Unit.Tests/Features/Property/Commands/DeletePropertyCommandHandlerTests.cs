using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RealStateApp.Core.Application.Exceptions;
using RealStateApp.Core.Application.Features.Property.Commands.DeleteProperty;
using RealStateApp.Core.Domain.Entities;
using RealStateApp.Infraestructure.Persistence.Contexts;
using RealStateApp.Infraestructure.Persistence.Repositories;

namespace RealStateApp.Unit.Tests.Features.Property.Commands
{
    public class DeletePropertyCommandHandlerTests
    {
        private readonly DbContextOptions<RealStateContext> _dbContextOptions;

        public DeletePropertyCommandHandlerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<RealStateContext>()
                .UseInMemoryDatabase(databaseName: $"RealStateAppTest_{Guid.NewGuid()}")
                .Options;
        }

        [Fact]
        public async Task Handle_ShouldDeleteProperty_WhenPropertyExists()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            
            var propertyType = new Core.Domain.Entities.PropertyType { Id = 1, Name = "Casa", Description = "Casa" };
            var saleType = new Core.Domain.Entities.SaleType { Id = 1, Name = "Venta", Description = "Venta" };
            
            await context.PropertyTypes.AddAsync(propertyType);
            await context.SaleTypes.AddAsync(saleType);
            
            var property = new Core.Domain.Entities.Property
            {
                Code = "DEL123",
                Price = 100000,
                SizeInMeters = 150.5,
                Bedrooms = 3,
                Bathrooms = 2,
                Description = "Property to delete",
                PropertyTypeId = 1,
                SaleTypeId = 1,
                AgentId = "agent123"
            };
            
            await context.Properties.AddAsync(property);
            await context.SaveChangesAsync();

            var propertyRepository = new PropertyRepository(context);
            var handler = new DeletePropertyCommandHandler(propertyRepository);

            var command = new DeletePropertyCommand { Code = "DEL123" };

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            var deletedProperty = await context.Properties.FirstOrDefaultAsync(p => p.Code == "DEL123");
            deletedProperty.Should().BeNull();
        }

        [Fact]
        public async Task Handle_ShouldThrowApiException_WhenPropertyDoesNotExist()
        {
            // Arrange
            using var context = new RealStateContext(_dbContextOptions);
            
            var propertyRepository = new PropertyRepository(context);
            var handler = new DeletePropertyCommandHandler(propertyRepository);

            var command = new DeletePropertyCommand { Code = "NOEXST" };

            // Act & Assert
            Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<ApiException>()
                .WithMessage("La propiedad no existe con ese código");
        }
    }
}
